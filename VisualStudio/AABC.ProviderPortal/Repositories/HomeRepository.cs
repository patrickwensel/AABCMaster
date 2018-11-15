using AABC.Domain.Cases;
using AABC.ProviderPortal.Models.Home;
using Dymeng.Framework.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace AABC.ProviderPortal.Repositories
{
    public class HomeRepository
    {

        private Data.Services.CaseService caseService;
        private string connectionString;
        private Data.V2.CoreContext _context;


        public HomeRepository(Data.V2.CoreContext context)
        {
            caseService = new Data.Services.CaseService();
            connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["CoreConnection"].ConnectionString;
            _context = context;
        }


        public List<CaseHoursDetailItem> GetCaseHoursDetails(int caseID, int providerID, DateTime? cutoff = null)
        {

            List<CaseHoursDetailItem> list = new List<CaseHoursDetailItem>();

            var caseAuthHours = caseService.GetCaseHoursByCase(caseID, cutoff)
                .Where(m => m.Status != AuthorizationHoursStatus.PreCheckedOnApp);

            var v2Provider = _context.Providers.Find(providerID);
            if (v2Provider.ProviderTypeID == (int)Domain2.Providers.ProviderTypeIDs.BoardCertifiedBehavioralAnalyst)
            {
                // don't filter
            }
            else
            {
                caseAuthHours = caseAuthHours.Where(x => x.ProviderID == providerID).ToList();
            }


            bool getExtendedNotes = false;
            if ((Global.Default.GetUserProvider().Type.Code == "BCBA") && (Domain.Hours.Note.UseExtendedNotes))
            {
                getExtendedNotes = true;
            }

            var context = new Data.Models.CoreEntityModel();

            foreach (var domainHours in caseAuthHours)
            {
                var hours = new CaseHoursDetailItem();
                hours.ID = domainHours.ID.Value;
                hours.Date = domainHours.Date;
                hours.EndTime = domainHours.TimeOut;
                hours.StartTime = domainHours.TimeIn;
                hours.Hours = domainHours.HoursTotal;
                hours.Service = domainHours.Service;
                hours.Notes = domainHours.Notes;
                hours.ProviderName = domainHours.Provider.FirstName + " " + domainHours.Provider.LastName;

                if (string.IsNullOrEmpty(hours.Notes) && getExtendedNotes)
                {
                    hours.Notes =
                        string.Join("; ",
                            context.CaseAuthHoursNotes
                                .Where(x => x.HoursID == hours.ID && !string.IsNullOrEmpty(x.NotesAnswer))
                                .Select(x => x.NotesAnswer).ToArray());
                }


                list.Add(hours);
            }

            return list.OrderByDescending(x => x.Date).ThenBy(x => x.StartTime).ToList();
        }


        public bool HasHoursInRange(int caseID, int providerID, DateTime date, DateTime timeIn, DateTime timeOut)
        {

            var count = _context.Hours
                .Where(x => x.CaseID == caseID
                    && x.ProviderID == providerID
                    && x.Date == date
                    && timeIn.TimeOfDay < x.EndTime
                    && timeOut.TimeOfDay > x.StartTime
                    ).Count();

            return count > 0;
        }

        public AuthorizationHoursStatus GetHoursStatus(int hoursID)
        {
            return (AuthorizationHoursStatus)_context.Hours.Find(hoursID).Status;
        }



















        internal int BeginFinalize(int caseID, DateTime firstDayOfMonth, int providerID)
        {
            int periodID = addOrGetCasePeriodID(caseID, firstDayOfMonth);

            using (SqlConnection conn = new SqlConnection(this.connectionString))
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.Connection = conn;

                cmd.Parameters.AddWithValue("@Period", periodID);
                cmd.Parameters.AddWithValue("@Provider", providerID);
                cmd.Parameters.AddWithValue("@Date", firstDayOfMonth);

                cmd.CommandText =
                    "SELECT ID FROM dbo.CaseMonthlyPeriodProviderFinalizations " +
                    "WHERE CaseMonthlyPeriodID = @Period AND ProviderID = @Provider";

                var existing = cmd.GetRowOrNull();
                if (existing != null)
                {
                    return existing.ToInt("ID");
                }

                cmd.CommandText =
                    "INSERT INTO dbo.CaseMonthlyPeriodProviderFinalizations (CaseMonthlyPeriodID, ProviderID, DateFinalized, IsComplete) " +
                    "VALUES (@Period, @Provider, @Date, 0);";

                return cmd.InsertToIdentity();

            }
        }

        internal void UpdateFinalizeWithEnvelopeID(int finalizationID, string envelopeID)
        {
            using (SqlConnection conn = new SqlConnection(this.connectionString))
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.Connection = conn;

                cmd.CommandText =
                    "UPDATE dbo.CaseMonthlyPeriodProviderFinalizations " +
                    "SET EnvelopeID = @envelopeID WHERE ID = @finalizationID";

                cmd.Parameters.AddWithValue("@envelopeID", envelopeID);
                cmd.Parameters.AddWithValue("@finalizationID", finalizationID);

                cmd.ExecuteNonQueryToInt();
            }
        }

        internal string GetEnvelopeIDForFinalization(int finalizationID)
        {
            using (SqlConnection conn = new SqlConnection(this.connectionString))
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.Connection = conn;

                cmd.CommandText =
                    "SELECT EnvelopeID FROM dbo.CaseMonthlyPeriodProviderFinalizations " +
                    "WHERE ID = @finalizationID";

                cmd.Parameters.AddWithValue("@finalizationID", finalizationID);

                return cmd.ExecuteScalarToString();
            }
        }

        internal void CompleteFinalize(int finalizationID)
        {
            using (SqlConnection conn = new SqlConnection(this.connectionString))
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.Connection = conn;

                cmd.CommandText =
                    "UPDATE dbo.CaseMonthlyPeriodProviderFinalizations " +
                    "SET IsComplete = 1, DateCreated = @date WHERE ID = @finalizationID";

                cmd.Parameters.AddWithValue("@finalizationID", finalizationID);
                cmd.Parameters.AddWithValue("@date", DateTime.Now);

                cmd.ExecuteNonQueryToInt();

                cmd.CommandText =
                    "SELECT CaseID, PeriodFirstDayOfMonth, ProviderID " +
                    "FROM[dbo].[CaseMonthlyPeriodProviderFinalizations] finalizations " +
                    "INNER JOIN[dbo].[CaseMonthlyPeriods] cmp " +
                    "ON finalizations.CaseMonthlyPeriodID = cmp.ID " +
                    "WHERE finalizations.ID = @finalizationID";

                var row = cmd.GetRow();
                var caseID = row.ToInt("CaseID");
                var periodFirstDayOfMonth = row.ToDateTime("PeriodFirstDayOfMonth");
                var providerID = row.ToInt("ProviderID");

                cmd.CommandText = "UPDATE cah SET cah.HoursStatus = 2 " +
                    "FROM dbo.CaseAuthHours AS cah " +
                    "LEFT JOIN dbo.CaseAuthCodes AS cac ON cac.ID = cah.CaseAuthID " +
                    "WHERE (cac.CaseID = @CaseID OR cah.CaseID = @CaseID) " +
                    "  AND cah.HoursDate >= @Start " +
                    "  AND cah.HoursDate <= @End " +
                    "  AND cah.HoursStatus = 1 " +
                    "  AND cah.CaseProviderID = @ProviderID " +
                    ";";

                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@CaseID", caseID);
                cmd.Parameters.AddWithValue("@Start", periodFirstDayOfMonth);
                cmd.Parameters.AddWithValue("@End", periodFirstDayOfMonth.AddMonths(1).AddDays(-1));
                cmd.Parameters.AddWithValue("@ProviderID", providerID);

                cmd.ExecuteNonQueryToInt();

            }
        }

        internal int GetHoursCountPerCaseProviderAndPeriod(int caseID, int providerID, DateTime firstDayOfMonth)
        {

            using (SqlConnection conn = new SqlConnection(this.connectionString))
            using (SqlCommand cmd = new SqlCommand())
            {

                cmd.Connection = conn;

                cmd.CommandText =
                    "SELECT COUNT(cah.ID) AS CountOfRows " +
                    "FROM dbo.CaseAuthHours AS cah " +
                    "LEFT JOIN dbo.CaseAuthCodes AS cac ON cac.ID = cah.CaseAuthID " +
                    "WHERE cah.CaseProviderID = @ProviderID " +
                    "  AND (cac.CaseID = @CaseID OR cah.CaseID = @CaseID) " +
                    "  AND cah.HoursDate >= @StartDate " +
                    "  AND cah.HoursDate <= @EndDate;";

                cmd.Parameters.AddWithValue("@CaseID", caseID);
                cmd.Parameters.AddWithValue("@ProviderID", providerID);
                cmd.Parameters.AddWithValue("@StartDate", firstDayOfMonth);
                cmd.Parameters.AddWithValue("@EndDate", firstDayOfMonth.AddMonths(1).AddDays(-1));

                return cmd.ExecuteScalarToInt();
            }

        }

        internal bool IsFinalized(int caseID, int providerID, DateTime firstDayOfMonth)
        {

            using (SqlConnection conn = new SqlConnection(this.connectionString))
            using (SqlCommand cmd = new SqlCommand())
            {

                cmd.Connection = conn;

                cmd.CommandText =
                    "SELECT pf.ID " +
                    "FROM dbo.CaseMonthlyPeriodProviderFinalizations AS pf " +
                    "INNER JOIN dbo.CaseMonthlyPeriods AS cp ON cp.ID = pf.CaseMonthlyPeriodID " +
                    "WHERE pf.ProviderID = @ProviderID " +
                    "  AND cp.CaseID = @CaseID " +
                    "  AND cp.PeriodFirstDayOfMonth = @FirstDayOfMonth;";

                cmd.Parameters.AddWithValue("@CaseID", caseID);
                cmd.Parameters.AddWithValue("@ProviderID", providerID);
                cmd.Parameters.AddWithValue("@FirstDayOfMonth", firstDayOfMonth);

                DataTable table = cmd.GetTable();

                if (table.Rows.Count == 0)
                {
                    return false;
                }
                else
                {
                    return true;
                }

            }

        }

        public int GetHoursOwner(int hoursID)
        {

            // return the ProviderID that this hour is entered under
            var hours = caseService.GetCaseHoursItem(hoursID);
            return hours.ProviderID;

        }

        int addOrGetCasePeriodID(int caseID, DateTime date)
        {

            var periods = caseService.GetCaseMonthlyPeriods(caseID, date, date);
            if (periods[0].ID.HasValue)
            {
                return periods[0].ID.Value;
            }
            else
            {

                using (SqlConnection conn = new SqlConnection(this.connectionString))
                using (SqlCommand cmd = new SqlCommand())
                {

                    cmd.Connection = conn;

                    cmd.CommandText = "INSERT INTO dbo.CaseMonthlyPeriods (CaseID, PeriodFirstDayOfMonth) VALUES (@CaseID, @Date);";

                    cmd.Parameters.AddWithValue("@CaseID", caseID);
                    cmd.Parameters.AddWithValue("@Date", date);

                    return cmd.InsertToIdentity();
                }
            }
        }

        internal void CommitPendingHours(int caseID, int providerID)
        {

            using (SqlConnection conn = new SqlConnection(this.connectionString))
            using (SqlCommand cmd = new SqlCommand())
            {

                cmd.Connection = conn;

                cmd.CommandText = "UPDATE cah SET cah.HoursStatus = @Status " +
                    "FROM dbo.CaseAuthHours AS cah LEFT JOIN dbo.CaseAuthCodes AS cac ON cac.ID = cah.CaseAuthID " +
                    "WHERE (CaseProviderID = @ProviderID) AND ((cac.CaseID = @CaseID) OR (cah.CaseID = @CaseID)) AND (cah.HoursStatus = 0);";

                cmd.Parameters.AddWithValue("@Status", Domain.Cases.AuthorizationHoursStatus.ComittedByProvider);
                cmd.Parameters.AddWithValue("@ProviderID", providerID);
                cmd.Parameters.AddWithValue("@CaseID", caseID);

                cmd.ExecuteNonQueryToInt();
            }

        }

        internal void DeleteCaseHours(int hoursID)
        {

            using (SqlConnection conn = new SqlConnection(this.connectionString))
            using (SqlCommand cmd = new SqlCommand())
            {

                cmd.Connection = conn;
                cmd.CommandText = "DELETE FROM dbo.CaseAuthHours WHERE ID = @ID OR HoursSSGParentID = @ID;";
                cmd.Parameters.AddWithValue("@ID", hoursID);

                cmd.ExecuteNonQueryToInt();
            }

        }

        public CaseProvider GetCaseProvider(int providerID, int caseID)
        {
            return caseService.GetCaseProviderByProviderAndCaseIDs(providerID, caseID);
        }

        public CaseGeneralHoursVM GetGeneralHours(int caseID)
        {

            var model = new CaseGeneralHoursVM();
            model.Items = new List<CaseGeneralHoursListItemVM>();

            using (SqlConnection conn = new SqlConnection(this.connectionString))
            using (SqlCommand cmd = new SqlCommand())
            {

                cmd.Connection = conn;

                string authsSql = "";
                authsSql += "SELECT cac.ID, cac.DateCreated, cac.AuthCodeID, cac.AuthClassID, ";
                authsSql += "  cac.AuthStartDate, cac.AuthEndDate, cac.AuthTotalHoursApproved, ";
                authsSql += "  ac.CodeCode, ac.CodeDescription, ";
                authsSql += "  cls.AuthClassCode, cls.AuthClassName, cls.AuthClassDescription ";
                authsSql += "FROM CaseAuthCodes AS cac ";
                authsSql += "INNER JOIN AuthCodes AS ac ON ac.ID = cac.AuthCodeID ";
                authsSql += "INNER JOIN CaseAuthClasses AS cls ON cac.AuthClassID = cls.ID ";
                authsSql += "WHERE cac.CaseID = @CaseID ";
                authsSql += "ORDER BY cac.AuthClassID ";
                authsSql += ";";

                string genHoursSql = "";
                genHoursSql += "SELECT gh.ID, gh.CaseAuthID, gh.HoursYear, gh.HoursMonth, gh.HoursApplied ";
                genHoursSql += "FROM dbo.CaseAuthCodeGeneralHours AS gh ";
                genHoursSql += "INNER JOIN dbo.CaseAuthCodes AS cac ON cac.ID = gh.CaseAuthID ";
                genHoursSql += "WHERE cac.CaseID = @CaseID ";
                genHoursSql += ";";

                cmd.CommandText = authsSql + genHoursSql;

                cmd.Parameters.AddWithValue("@CaseID", caseID);

                var set = cmd.GetDataSet(new string[] { "Auths", "GeneralHours" });

                var generalHours = new List<Domain.Cases.CaseAuthorizationGeneralHours>();
                foreach (DataRow r in set.Tables["GeneralHours"].Rows)
                {
                    var gh = new CaseAuthorizationGeneralHours();
                    gh.ID = r.ToInt("ID");
                    gh.Year = r.ToInt("HoursYear");
                    gh.Month = r.ToInt("HoursMonth");
                    gh.Hours = r.ToDouble("HoursApplied");
                    gh.CaseAuthID = r.ToInt("CaseAuthID");
                    generalHours.Add(gh);
                }

                foreach (DataRow r in set.Tables["Auths"].Rows)
                {

                    var item = new CaseGeneralHoursListItemVM();

                    item.AuthClass = new AuthorizationClass()
                    {
                        ID = r.ToInt("AuthClassID"),
                        Code = r.ToStringValue("AuthClassCode"),
                        Name = r.ToStringValue("AuthClassName"),
                        Description = r.ToStringValue("AuthClassDescription")
                    };

                    item.CaseAuthorizationID = r.ToInt("ID");
                    item.Code = r.ToStringValue("CodeCode");
                    item.Description = r.ToStringValue("CodeDescription");
                    item.EndDate = r.ToDateTime("AuthEndDate");
                    item.ID = r.ToInt("AuthCodeID");
                    item.StartDate = r.ToDateTime("AuthStartDate");
                    item.TotalHoursApproved = r.ToDouble("AuthTotalHoursApproved");

                    item.GeneralHours = new List<CaseAuthorizationGeneralHours>();
                    item.GeneralHours.AddRange(generalHours.Where(x => x.CaseAuthID.Value == item.CaseAuthorizationID.Value).ToList());

                    model.Items.Add(item);

                }

                return model;

            }

        }

        public List<Domain.Cases.Case> GetCaseListByProvider(int providerID)
        {
            return caseService.GetActiveCasesByProvider(providerID, AppService.Current.Settings.CaseVisibilityAfterEndDateDays);
        }

        public CaseHoursDetailItem GetCaseHoursDetail(int hoursID)
        {

            var item = new CaseHoursDetailItem();
            var hours = caseService.GetCaseHoursItem(hoursID);

            //item.AuthCode = hours.Authorization.Code;
            item.Date = hours.Date;
            item.EndTime = hours.TimeOut;
            item.Hours = hours.HoursTotal;
            item.ID = hours.ID.Value;
            item.Notes = hours.Notes;
            item.Service = hours.Service;
            item.StartTime = hours.TimeIn;
            item.CaseID = hours.CaseID.Value;

            return item;

        }


        internal FinalizeMonthPopupVM GetFinalizeMonthPopupVM(int caseID)
        {

            var model = new FinalizeMonthPopupVM();
            var items = new List<FinalizedMonthItem>();

            var data = caseService.GetCaseMonthlyPeriods(caseID, DateTime.Now.AddMonths(-6), DateTime.Now);

            foreach (var dataItem in data)
            {
                var item = new FinalizedMonthItem();
                item.ID = dataItem.ID;
                item.FirstDayOfTargetMonth = dataItem.FirstDayOfMonth;
                fillMonthlyFinalizationDateAndFinalized(item);
                items.Add(item);
            }

            model.Items = items.OrderByDescending(x => x.FirstDayOfTargetMonth).ToList();
            return model;

        }

        private void fillMonthlyFinalizationDateAndFinalized(FinalizedMonthItem item)
        {

            item.DateFinalized = null;
            item.IsFinalized = false;

            if (item.ID.HasValue)
            {

                using (SqlConnection conn = new SqlConnection(this.connectionString))
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = "SELECT DateCreated FROM dbo.CaseMonthlyPeriodProviderFinalizations " +
                        "WHERE CaseMonthlyPeriodID = @PeriodID AND ProviderID = @ProviderID AND IsComplete = 1;";

                    cmd.Parameters.AddWithValue("@PeriodID", item.ID.Value);
                    cmd.Parameters.AddWithValue("@ProviderID", Global.Default.GetUserProvider().ID.Value);

                    DataRow r = cmd.GetRowOrNull();

                    if (r == null)
                    {
                        return;
                    }
                    else
                    {
                        item.DateFinalized = r.ToDateTime("DateCreated");
                        item.IsFinalized = true;
                        return;
                    }

                }

            }

        }

        internal double GetTotalPendingHours(int caseID, int providerID)
        {

            System.Diagnostics.Debug.WriteLine("GET TOTAL PENDING HOURS FROM HOME REPOSITORY");

            using (SqlConnection conn = new SqlConnection(this.connectionString))
            using (SqlCommand cmd = new SqlCommand())
            {

                cmd.Connection = conn;

                cmd.CommandText = "SELECT SUM(cah.HoursTotal) AS TotalHours " +
                    "FROM dbo.CaseAuthHours AS cah LEFT JOIN dbo.CaseAuthCodes AS cac ON cac.ID = cah.CaseAuthID " +
                    "WHERE (CaseProviderID = @ProviderID) AND ((cac.CaseID = @CaseID) OR (cah.CaseID = @CaseID)) AND (cah.HoursStatus = 0);";

                cmd.Parameters.AddWithValue("@ProviderID", providerID);
                cmd.Parameters.AddWithValue("@CaseID", caseID);

                var row = cmd.GetRowOrNull();
                if (row == null)
                {
                    return 0;
                }
                else
                {
                    try
                    {
                        return row.ToDouble("TotalHours");
                    }
                    catch (Exception e)
                    {
                        string s = getDataRowValues(row);
                        Dymeng.Framework.Exceptions.LogMessageToTelementry(s + "(" + e.ToString() + ")");
                        return 0;
                    }
                }
            }
        }

        string getDataRowValues(DataRow row)
        {

            StringBuilder output = new StringBuilder();
            foreach (DataColumn col in row.Table.Columns)
            {
                output.AppendFormat("{0} ", row[col]);
            }
            return output.ToString();
        }

        private void resolveHoursAuths(int hoursID)
        {
            var c2 = new Data.V2.CoreContext();
            var h = c2.Hours.Find(hoursID);
            var authResolver = new DomainServices.Hours.AuthResolver(h);
            authResolver.UpdateAuths(c2);
        }

        public Case GetCase(int caseID)
        {
            return caseService.GetCase(caseID);
        }

        public List<CaseAuthorization> GetCaseAuthorizations(int caseID)
        {
            return caseService.GetCaseAuthorizationsAndHours(caseID);
        }

        public List<Service> GetServiceList(int providerTypeID, bool returnSSG = false)
        {
            return DomainServices.Providers.Services.GetServices(providerTypeID, returnSSG);
        }

        internal bool IsProviderSupervisor(int caseID, int providerID)
        {

            using (SqlConnection conn = new SqlConnection(this.connectionString))
            using (SqlCommand cmd = new SqlCommand())
            {

                cmd.Connection = conn;

                cmd.CommandText = "SELECT IsSupervisor FROM dbo.CaseProviders WHERE CaseID = @CaseID AND ProviderID = @ProviderID;";

                cmd.Parameters.AddWithValue("@CaseID", caseID);
                cmd.Parameters.AddWithValue("@ProviderID", providerID);

                var row = cmd.GetRowOrNull();

                if (row == null)
                {
                    return false;
                }
                else
                {
                    return row.ToBool("IsSupervisor");
                }

            }

        }











    }
}