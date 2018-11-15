using AABC.Data.V2;
using AABC.Domain.Cases;
using AABC.Domain.Services;
using AABC.DomainServices.Staffing;
using AABC.Web.App.Patients;
using AABC.Web.App.Providers;
using AABC.Web.Models.Cases;
using AABC.Web.Models.Providers;
using Dymeng.Framework;
using Dymeng.Framework.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Caching;

namespace AABC.Web.Repositories
{

    public interface ICaseRepository
    {
        List<Service> GetAllServices();
        List<Service> GetAllServicesExceptSSG();
        GeneralHoursVM GetGeneralHours(int caseID);
        GeneralHoursEditVM GetGeneralHoursEdit(int caseAuthID);
        void DischargeCase(int caseID, string dischargeNotes);
        List<Authorization> GetAuthorizationsList();
        List<AuthorizationClass> GetAuthClassList();
        List<Authorization> GetAuthorizationsList(int caseID);
        void CreateAuthorization(string code, string description);
        List<CaseAuthorization> GetCaseAuthorizations(int caseID);
        List<TimeBillGridAuthItemVM> GetCaseAuthorizationsForTimeBillGrid(int caseID);
        List<CaseAuthorization> GetCaseAuthorizationsWithGeneralHours(int caseID);
        void SaveCaseAuthEntry(AuthCreateVM model);
        void SaveGeneralHoursEdit(GeneralHoursEditVM model);
        void CaseManageTimeBillBatchUpdate(int caseID, List<TimeBillGridListItemVM> items);
        void DeleteCaseAuthorization(int caseID, int authID);
        SummaryVM GetCaseManagementSummary(int caseID);
        void SaveCaseManagementSummary(SummaryVM model);
        ProvidersVM GetCaseManagementProviders(int caseID);
        List<CaseStatusDescription> GetCaseStatusDescriptionList();
        List<Domain.OfficeStaff.OfficeStaff> GetOfficeStaffList();
        List<ProvidersListItemVM> GetAllProvidersDropdownList();
        void SaveCaseManagementProvidersEdit(List<CaseProvider> providers, int caseID);
        List<CaseProvider> GetCaseManagementProvidersList(int caseID);
        string GetPatientNameByCaseID(int caseID);
        void CaseManageTimeBillBatchDelete(List<int> deleteKeys);
        void AddCaseHoursViaAdmin(int caseID, int providerID, DateTime date, DateTime timeIn, DateTime timeOut, int serviceID, string notes, bool isAdjustment);
        void DefinalizeProvider(int caseID, int providerID, DateTime firstDayOfPeriod);
        List<ServiceLocation> GetServiceLocationList();
        IEnumerable<Domain2.Cases.FunctioningLevel> GetFunctioningLevelList();
        void Reactivate(int caseID);
    }


    public class CaseRepository : ICaseRepository
    {

        public List<Authorization> GetAuthorizationsList(int caseID)
        {
            var c = Context.Cases.Find(caseID);
            if (c.Insurances.Count == 0)
            {
                return new List<Authorization>();
            }
            List<int> authIDs = new List<int>();
            foreach (var insurance in c.Insurances)
            {
                authIDs.AddRange(insurance.Insurance.AuthorizationMatchRules
                    .Where(x => x.FinalAuthorizationID.HasValue)
                    .Select(x => x.FinalAuthorizationID.Value).ToList());

                authIDs.AddRange(insurance.Insurance.AuthorizationMatchRules
                    .Where(x => x.InitialAuthorizationID.HasValue)
                    .Select(x => x.InitialAuthorizationID.Value).ToList());
            }
            authIDs = authIDs.Distinct().ToList();
            var auths = from auth in Context.AuthorizationCode
                        join matched in authIDs on auth.ID equals matched
                        select auth;
            var items = new List<Authorization>();
            foreach (var auth in auths.Distinct())
            {
                items.Add(new Authorization()
                {
                    Code = auth.Code,
                    DateCreated = auth.DateCreated,
                    Description = auth.Description,
                    ID = auth.ID
                });
            }
            return items;
        }


        public void Reactivate(int caseID)
        {

        }


        public List<ServiceLocation> GetServiceLocationList()
        {
            return new Data.Services.ServicesService().GetActiveServiceLocations();
        }


        public void DefinalizeProvider(int caseID, int providerID, DateTime firstDayOfPeriod)
        {
            using (SqlConnection conn = new SqlConnection(this.connectionString))
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.Connection = conn;
                cmd.CommandText =
                    "DELETE f " +
                    "FROM dbo.CaseMonthlyPeriodProviderFinalizations AS f " +
                    "INNER JOIN dbo.CaseMonthlyPeriods AS p ON p.ID = f.CaseMonthlyPeriodID " +
                    "WHERE p.CaseID = @CaseID " +
                    "  AND f.ProviderID = @ProviderID " +
                    "  AND CONVERT(DATE, p.PeriodFirstDayOfMonth) = @FirstDayOfMonth;";
                cmd.Parameters.AddWithValue("@CaseID", caseID);
                cmd.Parameters.AddWithValue("@ProviderID", providerID);
                cmd.Parameters.Add("@FirstDayOfMonth", SqlDbType.DateTime2);
                cmd.Parameters["@FirstDayOfMonth"].SqlValue = firstDayOfPeriod;
                var count = cmd.ExecuteNonQueryToInt();
                cmd.Parameters.Add("@FirstDayOfNextMonth", SqlDbType.DateTime2);
                cmd.Parameters["@FirstDayOfNextMonth"].SqlValue = firstDayOfPeriod.AddMonths(1);
                cmd.CommandText = "UPDATE dbo.CaseAuthHours SET HoursStatus = 1 " +
                    "WHERE CaseProviderID = @ProviderID AND CaseID = @CaseID AND (HoursDate >= @FirstDayOfMOnth AND HoursDate < @FirstDayOfNextMonth);";
                cmd.ExecuteNonQueryToInt();
            }
        }


        public void AddCaseHoursViaAdmin(int caseID, int providerID, DateTime date, DateTime timeIn, DateTime timeOut, int serviceID, string notes, bool isAdjustment)
        {
            using (SqlConnection conn = new SqlConnection(this.connectionString))
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.Connection = conn;
                cmd.CommandText = "INSERT INTO dbo.CaseAuthHours " +
                    "(CaseID, CaseProviderID, HoursDate, HoursTimeIn, HoursTimeOut, HoursTotal, HoursServiceID, HoursNotes, HoursStatus, HoursBillable, HoursPayable, IsPayrollOrBillingAdjustment) VALUES (" +
                    "@CaseID, @ProviderID, @Date, @TimeIn, @TimeOut, @Total, @ServiceID, @Notes, @Status, @Billable, @Payable, @IsAdjustment);";
                timeIn = new DateTime(date.Year, date.Month, date.Day, timeIn.Hour, timeIn.Minute, 0);
                timeOut = new DateTime(date.Year, date.Month, date.Day, timeOut.Hour, timeOut.Minute, 0);
                TimeSpan diff = timeOut - timeIn;
                double total = diff.TotalHours;
                cmd.Parameters.AddWithValue("@CaseID", caseID);
                cmd.Parameters.AddWithValue("@ProviderID", providerID);
                cmd.Parameters.AddWithValue("@Date", date);
                cmd.Parameters.AddWithValue("@TimeIn", timeIn);
                cmd.Parameters.AddWithValue("@TimeOut", timeOut);
                cmd.Parameters.AddWithValue("@Total", total);
                cmd.Parameters.AddWithValue("@ServiceID", serviceID);
                cmd.Parameters.AddWithValue("@Notes", notes);
                cmd.Parameters.AddWithValue("@Status", Domain.Cases.AuthorizationHoursStatus.FinalizedByProvider);
                cmd.Parameters.AddWithValue("@Billable", total);
                cmd.Parameters.AddWithValue("@Payable", total);
                cmd.Parameters.AddWithValue("@IsAdjustment", isAdjustment);
                var id = cmd.InsertToIdentity();
                ResolveHoursAuths(id);
            }
        }


        private void ResolveHoursAuths(int hoursID)
        {
            var h = Context.Hours.Find(hoursID);
            var authResolver = new DomainServices.Hours.AuthResolver(h);
            authResolver.UpdateAuths(Context);
        }


        public void CaseManageTimeBillBatchDelete(List<int> deleteKeys)
        {
            using (SqlConnection conn = new SqlConnection(this.connectionString))
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.Connection = conn;
                cmd.CommandText = "DELETE FROM dbo.CaseAuthHours WHERE ID = @ID;";
                cmd.Parameters.Add("@ID", SqlDbType.Int);
                try
                {
                    cmd.Connection.Open();
                    foreach (int k in deleteKeys)
                    {
                        cmd.Parameters[0].Value = k;
                        cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception e)
                {
                    throw e;
                }
                finally
                {
                    cmd.Connection.Close();
                }
            }
        }


        public void CaseManageTimeBillBatchUpdate(int caseID, List<TimeBillGridListItemVM> items)
        {
            using (SqlConnection conn = new SqlConnection(this.connectionString))
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.Connection = conn;
                string sql = "";
                sql += "UPDATE dbo.CaseAuthHours SET ";
                sql += "CaseAuthID = @AuthID, ";
                sql += "HoursDate = @Date, ";
                sql += "HoursTimeIn = @TimeIn, ";
                sql += "HoursTimeOut = @TimeOut, ";
                sql += "HoursTotal = @Total, ";
                sql += "HoursServiceID = @ServiceID, ";
                sql += "CaseID = @CaseID, ";
                sql += "HoursStatus = @Status, ";
                sql += "HoursBillable = @Billable, ";
                sql += "HoursPayable = @Payable, ";
                sql += "HoursHasCatalystData = @HasData, ";
                sql += "ServiceLocationID = @ServiceLocation ";
                sql += "WHERE ID = @ID;";
                cmd.CommandText = sql;
                foreach (var item in items)
                {
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithNullableValue("@AuthID", item.CaseAuth?.ID);
                    cmd.Parameters.AddWithValue("@Date", item.Date);
                    cmd.Parameters.AddWithValue("@TimeIn", item.TimeIn);
                    cmd.Parameters.AddWithValue("@TimeOut", item.TimeOut);
                    cmd.Parameters.AddWithValue("@Total", item.Hours);
                    cmd.Parameters.AddWithNullableValue("@ServiceID", item.Service?.ID);
                    cmd.Parameters.AddWithValue("@CaseID", caseID);
                    cmd.Parameters.AddWithValue("@Status", Domain.Cases.AuthorizationHoursStatus.ScrubbedByAdmin);
                    cmd.Parameters.AddWithNullableValue("@Billable", item.Billable);
                    cmd.Parameters.AddWithNullableValue("@Payable", item.Payable);
                    cmd.Parameters.AddWithValue("@HasData", item.HasCatalystData);
                    cmd.Parameters.AddWithValue("@ID", item.HoursID);
                    cmd.Parameters.AddWithNullableValue("@ServiceLocation", item.ServiceLocation?.ID);
                    cmd.ExecuteNonQueryToInt();
                    ResolveHoursAuths(item.HoursID);
                }
            }
        }


        public List<Service> GetAllServicesExceptSSG()
        {
            return DomainServices.Providers.Services.GetServices(null);
        }


        public List<Service> GetAllServices()
        {
            return new Data.Services.CaseService().GetServices().ToList();
        }


        string GetTimeBillStatusCodeDisplay(AuthorizationHoursStatus status)
        {
            switch (status)
            {
                case AuthorizationHoursStatus.FinalizedByProvider:
                    return "E";
                case AuthorizationHoursStatus.ScrubbedByAdmin:
                    return "F";
                default:
                    return "F";
            }
        }


        public List<CaseAuthorization> GetCaseAuthorizationsWithGeneralHours(int caseID)
        {
            return new Data.Services.CaseService().GetCaseAuthorizationsAndGeneralHours(caseID);
        }


        public void DischargeCase(int caseID, string dischargeNotes)
        {
            using (SqlConnection conn = new SqlConnection(this.connectionString))
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.Connection = conn;
                cmd.CommandText = "UPDATE dbo.Cases SET CaseStatus = -1, CaseDischargeNotes = @Notes WHERE ID = @ID;";
                cmd.Parameters.AddWithValue("ID", caseID);
                cmd.Parameters.AddWithNullableValue("@Notes", dischargeNotes);
                cmd.ExecuteNonQueryToInt();
            }
            RefreshPatientCache(caseID);
        }


        public string GetPatientNameByCaseID(int caseID)
        {
            using (SqlConnection conn = new SqlConnection(this.connectionString))
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.Connection = conn;
                cmd.CommandText =
                    "SELECT p.PatientFirstname, p.PatientLastName " +
                    "FROM dbo.Cases AS c INNER JOIN dbo.Patients AS p ON p.ID = c.PatientID " +
                    "WHERE c.ID = @CaseID;";
                cmd.Parameters.AddWithValue("@CaseID", caseID);
                var row = cmd.GetRow();
                return row.ToStringValue("PatientLastName") + ", " + row.ToStringValue("PatientFirstName");
            }
        }


        public void CreateAuthorization(string code, string description)
        {
            using (SqlConnection conn = new SqlConnection(this.connectionString))
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.Connection = conn;
                cmd.CommandText = "INSERT INTO AuthCodes (CodeCode, CodeDescription) VALUES (@Code, @Description);";
                cmd.Parameters.AddWithValue("@Code", code);
                cmd.Parameters.AddWithNullableValue("@Description", description);
                cmd.InsertToIdentity();
            }
        }


        public List<TimeBillGridAuthItemVM> GetCaseAuthorizationsForTimeBillGrid(int caseID)
        {
            using (SqlConnection conn = new SqlConnection(this.connectionString))
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.Connection = conn;
                cmd.CommandText =
                    "SELECT cac.ID, cl.AuthClassCode, ac.CodeCode, cac.AuthStartDate, cac.AuthEndDate, ac.ID AS AuthID, cl.ID AS ClassID " +
                    "FROM dbo.CaseAuthCodes AS cac " +
                    "INNER JOIN dbo.AuthCodes AS ac ON ac.ID = cac.AuthCodeID " +
                    "INNER JOIN dbo.CaseAuthClasses AS cl ON cac.AuthClassID = cl.ID " +
                    "WHERE cac.CaseID = @CaseID;";
                cmd.Parameters.AddWithValue("@CaseID", caseID);
                var table = cmd.GetTable();
                var list = new List<TimeBillGridAuthItemVM>();
                foreach (DataRow r in table.Rows)
                {
                    var item = new TimeBillGridAuthItemVM
                    {
                        ID = r.ToInt("ID"),
                        ClassID = r.ToInt("ClassID"),
                        AuthID = r.ToInt("AuthID"),
                        Class = r.ToStringValue("AuthClassCode"),
                        Code = r.ToStringValue("CodeCode"),
                        End = r.ToDateTime("AuthEndDate"),
                        Start = r.ToDateTime("AuthStartDate")
                    };
                    list.Add(item);
                }
                return list;
            }
        }


        public List<CaseAuthorization> GetCaseAuthorizations(int caseID)
        {
            using (SqlConnection conn = new SqlConnection(this.connectionString))
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.Connection = conn;
                cmd.CommandText = "SELECT ID, AuthCodeID, AuthClassID, AuthStartDate, AuthEndDate, AuthTotalHoursApproved " +
                    "FROM dbo.CaseAuthCodes WHERE CaseID = @CaseID;";
                cmd.Parameters.AddWithValue("@CaseID", caseID);
                var table = cmd.GetTable();
                var list = new List<CaseAuthorization>();
                foreach (DataRow r in table.Rows)
                {
                    var ca = new CaseAuthorization
                    {
                        ID = r.ToInt("ID"),
                        AuthClass = new AuthorizationClass() { ID = r.ToInt("AuthClassID") },
                        Code = r.ToStringValue("AuthCodeID"),
                        StartDate = r.ToDateTime("AuthStartDate"),
                        EndDate = r.ToDateTime("AuthEndDate"),
                        TotalHoursApproved = r.ToDouble("AuthTotalHoursApproved")
                    };
                    list.Add(ca);
                }
                return list;
            }
        }


        public void SaveCaseAuthEntry(AuthCreateVM model)
        {
            using (SqlConnection conn = new SqlConnection(this.connectionString))
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.Connection = conn;
                cmd.CommandText = "INSERT INTO dbo.CaseAuthCodes (CaseID, AuthCodeID, AuthClassID, AuthStartDate, AuthEndDate, AuthTotalHoursApproved) " +
                    "VALUES (@CaseID, @CodeID, @ClassID, @StartDate, @EndDate, @TotalHoursApproved);";
                cmd.Parameters.AddWithValue("@CaseID", model.ViewHelper.CaseID.Value);
                cmd.Parameters.AddWithValue("@CodeID", model.Detail.ID.Value);
                cmd.Parameters.AddWithValue("@ClassID", model.Detail.AuthClass.ID.Value);
                cmd.Parameters.AddWithNullableValue("@StartDate", model.Detail.StartDate);
                cmd.Parameters.AddWithNullableValue("@EndDate", model.Detail.EndDate);
                cmd.Parameters.AddWithValue("@TotalHoursApproved", model.Detail.TotalHoursApproved);
                cmd.ExecuteNonQueryToInt();
                RefreshPatientCache(model.ViewHelper.CaseID.Value);
            }
        }


        public void SaveGeneralHoursEdit(GeneralHoursEditVM model)
        {
            if (model.Detail.CaseAuthorizationID.HasValue)
            {
                UpdateExistingCaseAuth(model.Detail);
            }
            else
            {
                SaveNewCaseAuth(model.Detail, model.ViewHelper.CaseID.Value);
            }
            PatientRepository.RecalculateCaseStatus(model.ViewHelper.CaseID.Value);
            RefreshPatientCache(model.ViewHelper.CaseID.Value);
        }


        private void UpdateExistingCaseAuth(CaseAuthorization model)
        {
            using (SqlConnection conn = new SqlConnection(this.connectionString))
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.Connection = conn;
                string sql = "";
                sql += "UPDATE dbo.CaseAuthCodes SET ";
                sql += "  AuthCodeID = @CodeID, ";
                sql += "  AuthClassID = @ClassID, ";
                sql += "  AuthStartDate = @StartDate, ";
                sql += "  AuthEndDate = @EndDate, ";
                sql += "  AuthTotalHoursApproved = @TotalHoursApproved ";
                sql += "WHERE ID = @CaseAuthID ";
                sql += ";";
                cmd.CommandText = sql;
                cmd.Parameters.AddWithValue("@CaseAuthID", model.CaseAuthorizationID.Value);
                cmd.Parameters.AddWithValue("@CodeID", model.ID.Value);
                cmd.Parameters.AddWithValue("@ClassID", model.AuthClass.ID.Value);
                cmd.Parameters.AddWithNullableValue("@StartDate", model.StartDate);
                cmd.Parameters.AddWithNullableValue("@EndDate", model.EndDate);
                cmd.Parameters.AddWithValue("@TotalHoursApproved", model.TotalHoursApproved);
                cmd.ExecuteNonQueryToInt();
            }
        }


        private void SaveNewCaseAuth(CaseAuthorization model, int caseID)
        {
            using (SqlConnection conn = new SqlConnection(this.connectionString))
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.Connection = conn;
                string sql = "";
                sql += "INSERT INTO dbo.CaseAuthCodes (";
                sql += "  CaseID, AuthCodeID, AuthClassID, AuthStartDate, AuthEndDate, AuthTotalHoursApproved ";
                sql += ") VALUES (";
                sql += "  @CaseID, @CodeID, @ClassID, @StartDate, @EndDate, @TotalHoursApproved ";
                sql += ")";
                sql += ";";
                cmd.CommandText = sql;
                cmd.Parameters.AddWithValue("@CaseID", caseID);
                cmd.Parameters.AddWithValue("@CodeID", model.ID.Value);
                cmd.Parameters.AddWithValue("@ClassID", model.AuthClass.ID.Value);
                cmd.Parameters.AddWithNullableValue("@StartDate", model.StartDate);
                cmd.Parameters.AddWithNullableValue("@EndDate", model.EndDate);
                cmd.Parameters.AddWithValue("@TotalHoursApproved", model.TotalHoursApproved);
                model.CaseAuthorizationID = cmd.InsertToIdentity();
            }
        }


        public void DeleteCaseAuthorization(int caseID, int authID)
        {
            using (SqlConnection conn = new SqlConnection(this.connectionString))
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.Connection = conn;
                cmd.CommandText = "DELETE FROM dbo.CaseAuthCodes WHERE ID = @AuthID;";
                cmd.Parameters.AddWithValue("@AuthID", authID);
                cmd.ExecuteNonQueryToInt();
                PatientRepository.RecalculateCaseStatus(caseID);
                RefreshPatientCache(caseID);
            }

        }


        public List<Authorization> GetAuthorizationsList()
        {

            using (SqlConnection conn = new SqlConnection(this.connectionString))
            using (SqlCommand cmd = new SqlCommand())
            {

                cmd.Connection = conn;

                cmd.CommandText = "SELECT ID, DateCreated, CodeCode, CodeDescription FROM dbo.AuthCodes;";

                var table = cmd.GetTable();

                var list = new List<Authorization>();

                foreach (DataRow r in table.Rows)
                {
                    var auth = new Authorization
                    {
                        ID = r.ToInt("ID"),
                        DateCreated = r.ToDateTime("DateCreated"),
                        Code = r.ToStringValue("CodeCode"),
                        Description = r.ToStringValue("CodeDescription")
                    };
                    list.Add(auth);
                }

                return list;
            }
        }


        public List<AuthorizationClass> GetAuthClassList()
        {
            var mc = MemoryCache.Default;
            if (!mc.Contains("CaseAuthClassList"))
            {
                var exp = DateTimeOffset.UtcNow.AddMinutes(30);
                mc.Add("CaseAuthClassList", _GetAuthClassList(), exp);
            }
            return mc.Get("CaseAuthClassList") as List<AuthorizationClass>;
        }


        private List<AuthorizationClass> _GetAuthClassList()
        {
            using (SqlConnection conn = new SqlConnection(this.connectionString))
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.Connection = conn;
                cmd.CommandText = "SELECT ID, DateCreated, AuthClassCode, AuthClassName, AuthClassDescription FROM dbo.CaseAuthClasses;";
                var table = cmd.GetTable();
                var list = new List<AuthorizationClass>();
                foreach (DataRow r in table.Rows)
                {
                    var ac = new AuthorizationClass
                    {
                        ID = r.ToInt("ID"),
                        Code = r.ToStringValue("AuthClassCode"),
                        Name = r.ToStringValue("AuthClassName"),
                        Description = r.ToStringValue("AuthClassDescription")
                    };
                    list.Add(ac);
                }
                return list;
            }
        }


        public GeneralHoursEditVM GetGeneralHoursEdit(int caseAuthID)
        {
            var model = new GeneralHoursEditVM
            {
                Detail = new CaseAuthorization()
            };
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
                authsSql += "WHERE cac.ID = @CaseAuthID ";
                authsSql += ";";

                string genHoursSql = "";
                genHoursSql += "SELECT gh.ID, gh.CaseAuthID, gh.HoursYear, gh.HoursMonth, gh.HoursApplied ";
                genHoursSql += "FROM dbo.CaseAuthCodeGeneralHours AS gh ";
                genHoursSql += "INNER JOIN dbo.CaseAuthCodes AS cac ON cac.ID = gh.CaseAuthID ";
                genHoursSql += "WHERE cac.ID = @CaseAuthID ";
                genHoursSql += ";";

                cmd.CommandText = authsSql + genHoursSql;
                cmd.Parameters.AddWithValue("@CaseAuthID", caseAuthID);
                var set = cmd.GetDataSet(new string[] { "Auth", "GeneralHours" });
                {
                    DataRow r = set.Tables["Auth"].Rows[0];
                    model.Detail.AuthClass = new AuthorizationClass()
                    {
                        ID = r.ToInt("AuthClassID"),
                        Code = r.ToStringValue("AuthClassCode"),
                        Name = r.ToStringValue("AuthClassName"),
                        Description = r.ToStringValue("AuthClassDescription")
                    };
                    model.Detail.CaseAuthorizationID = r.ToInt("ID");
                    model.Detail.Code = r.ToStringValue("CodeCode");
                    model.Detail.Description = r.ToStringValue("CodeDescription");
                    model.Detail.EndDate = r.ToDateTime("AuthEndDate");
                    model.Detail.ID = r.ToInt("AuthCodeID");
                    model.Detail.StartDate = r.ToDateTime("AuthStartDate");
                    model.Detail.TotalHoursApproved = r.ToDouble("AuthTotalHoursApproved");
                }
                model.Detail.GeneralHours = new List<CaseAuthorizationGeneralHours>();

                var generalHours = new List<CaseAuthorizationGeneralHours>();
                foreach (DataRow r in set.Tables["GeneralHours"].Rows)
                {
                    var gh = new CaseAuthorizationGeneralHours
                    {
                        ID = r.ToInt("ID"),
                        Year = r.ToInt("HoursYear"),
                        Month = r.ToInt("HoursMonth"),
                        Hours = r.ToDouble("HoursApplied"),
                        CaseAuthID = r.ToInt("CaseAuthID")
                    };
                    model.Detail.GeneralHours.Add(gh);
                }
                return model;
            }
        }


        public GeneralHoursVM GetGeneralHours(int caseID)
        {

            var model = new GeneralHoursVM
            {
                Items = new List<GeneralHoursListItemVM>()
            };

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
                    var gh = new CaseAuthorizationGeneralHours
                    {
                        ID = r.ToInt("ID"),
                        Year = r.ToInt("HoursYear"),
                        Month = r.ToInt("HoursMonth"),
                        Hours = r.ToDouble("HoursApplied"),
                        CaseAuthID = r.ToInt("CaseAuthID")
                    };
                    generalHours.Add(gh);
                }

                foreach (DataRow r in set.Tables["Auths"].Rows)
                {

                    var item = new GeneralHoursListItemVM
                    {
                        AuthClass = new AuthorizationClass()
                        {
                            ID = r.ToInt("AuthClassID"),
                            Code = r.ToStringValue("AuthClassCode"),
                            Name = r.ToStringValue("AuthClassName"),
                            Description = r.ToStringValue("AuthClassDescription")
                        },

                        CaseAuthorizationID = r.ToInt("ID"),
                        Code = r.ToStringValue("CodeCode"),
                        Description = r.ToStringValue("CodeDescription"),
                        EndDate = r.ToDateTime("AuthEndDate"),
                        ID = r.ToInt("AuthCodeID"),
                        StartDate = r.ToDateTime("AuthStartDate"),
                        TotalHoursApproved = r.ToDouble("AuthTotalHoursApproved"),

                        GeneralHours = new List<CaseAuthorizationGeneralHours>()
                    };
                    item.GeneralHours.AddRange(generalHours.Where(x => x.CaseAuthID.Value == item.CaseAuthorizationID.Value).ToList());

                    model.Items.Add(item);

                }

                return model;

            }

        }


        /******************
            CORE VIEWS (GET)
        ******************/
        public SummaryVM GetCaseManagementSummary(int caseID)
        {
            return GetCaseSummary(caseID);
        }


        public ProvidersVM GetCaseManagementProviders(int caseID)
        {
            return GetCaseManagementProvider(caseID);
        }


        private Domain.Patients.Patient GetPatientNameInfo(int caseID)
        {

            using (SqlConnection conn = new SqlConnection(this.connectionString))
            using (SqlCommand cmd = new SqlCommand())
            {

                cmd.Connection = conn;

                Domain.Patients.Patient patient = new Domain.Patients.Patient();

                string patientSql = "";
                patientSql += "SELECT p.ID, p.PatientFirstName, p.PatientLastName, p.PatientDateOfBirth, ";
                patientSql += "  p.PatientGuardianFirstName, p.PatientGuardianLastName, p.PatientGuardianRelationship, p.HighRisk ";
                patientSql += "FROM dbo.Patients AS p ";
                patientSql += "INNER JOIN dbo.Cases AS c ON c.PatientID = p.ID ";
                patientSql += "WHERE c.ID = @CaseID ";
                patientSql += ";";

                cmd.CommandText = patientSql;
                cmd.Parameters.AddWithValue("@CaseID", caseID);

                var r = cmd.GetRow();

                patient.ID = r.ToInt("ID");
                patient.FirstName = r.ToStringValue("PatientFirstName");
                patient.LastName = r.ToStringValue("PatientLastName");
                patient.DateOfBirth = r.ToDateTimeOrNull("PatientDateOfBirth");
                patient.GuardianFirstName = r.ToStringValue("PatientGuardianFirstName");
                patient.GuardianLastName = r.ToStringValue("PatientGuardianLastName");
                patient.GuardianRelationship = r.ToStringValue("PatientGuardianRelationship");
                patient.HighRisk = r.ToBool("HighRisk");
                return patient;
            }

        }


        public List<CaseProvider> GetCaseManagementProvidersList(int caseID)
        {

            var providers = new List<CaseProvider>();

            using (SqlConnection conn = new SqlConnection(this.connectionString))
            using (SqlCommand cmd = new SqlCommand())
            {

                cmd.Connection = conn;

                string providersSql = "";
                providersSql += "SELECT p.ID, p.ProviderType, p.ProviderFirstName, p.ProviderLastName, p.ProviderPrimaryEmail, p.ProviderPrimaryPhone, cp.Active, cp.ID AS CaseProviderID, ";
                providersSql += "  pt.ID AS ProviderTypeID, pt.ProviderTypeCode, pt.ProviderTypeName, pt.ProviderTypeCanSuperviseCase, pt.ProviderTypeIsOutsourced, ";
                providersSql += "  cp.IsSupervisor, cp.IsAssessor ";
                providersSql += "FROM dbo.Providers AS p ";
                providersSql += "INNER JOIN dbo.CaseProviders AS cp ON cp.ProviderID = p.ID ";
                providersSql += "LEFT JOIN dbo.ProviderTypes AS pt ON pt.ID = p.ProviderType ";
                providersSql += "WHERE cp.CaseID = @CaseID ";
                providersSql += ";";

                cmd.CommandText = providersSql;

                cmd.Parameters.AddWithValue("@CaseID", caseID);

                try
                {

                    var table = cmd.GetTable();

                    foreach (DataRow r in table.Rows)
                    {

                        var p = new CaseProvider
                        {
                            ID = r.ToInt("ID"),
                            CaseProviderID = r.ToInt("CaseProviderID"),
                            Active = r.ToBool("Active"),
                            FirstName = r.ToStringValue("ProviderFirstName"),
                            LastName = r.ToStringValue("ProviderLastName"),
                            Email = r.ToStringValue("ProviderPrimaryEmail"),
                            Phone = r.ToStringValue("ProviderPrimaryPhone"),
                            Type = new Domain.Providers.ProviderType()
                        };
                        if (r.ToIntOrNull("ProviderTypeID") != null)
                        {
                            p.Type.ID = r.ToInt("ProviderTypeID");
                            p.Type.Name = r.ToStringValue("ProviderTypeName");
                            p.Type.Code = r.ToStringValue("ProviderTypeCode");
                            p.Type.CanSuperviseCase = r.ToBool("ProviderTypeCanSuperviseCase");
                            p.Type.IsOutsourced = r.ToBool("ProviderTypeIsOutsourced");
                            p.Supervisor = r.ToBool("IsSupervisor");
                            p.Assessor = r.ToBool("IsAssessor");
                        }

                        providers.Add(p);
                    }

                    return providers;

                }
                catch (Exception e)
                {
                    Exceptions.Handle(e);
                    throw e;
                }
            }
        }


        /******************
            CORE VIEWS (SAVE)
        ******************/
        public void SaveCaseManagementProvidersEdit(List<CaseProvider> providers, int caseID)
        {
            using (SqlConnection conn = new SqlConnection(this.connectionString))
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.Connection = conn;
                foreach (var p in providers)
                {
                    if (p.CaseProviderID.HasValue)
                    {
                        // update
                        string sql = "";
                        sql += "UPDATE dbo.CaseProviders SET Active = @Active, IsSupervisor = @IsSuper, IsAssessor = @Assessor ";
                        sql += "WHERE ID = @ID ";
                        sql += ";";
                        cmd.CommandText = sql;
                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("@Active", p.Active);
                        cmd.Parameters.AddWithValue("@IsSuper", p.Supervisor);
                        cmd.Parameters.AddWithValue("@ID", p.CaseProviderID);
                        cmd.Parameters.AddWithValue("@Assessor", p.Assessor);
                        try
                        {
                            cmd.ExecuteNonQueryToInt();
                        }
                        catch (Exception e)
                        {
                            Exceptions.Handle(e);
                            throw e;
                        }
                    }
                    else
                    {
                        // insert
                        string sql = "";
                        sql += "INSERT INTO dbo.CaseProviders (";
                        sql += "  CaseID, ProviderID, Active, IsSupervisor, IsAssessor ";
                        sql += ") VALUES (@CaseID, @ProviderID, @Active, @IsSuper, @Assessor) ";
                        sql += "; ";
                        cmd.CommandText = sql;
                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("@Active", p.Active);
                        cmd.Parameters.AddWithValue("@IsSuper", p.Supervisor);
                        cmd.Parameters.AddWithValue("@Assessor", p.Assessor);
                        cmd.Parameters.AddWithValue("@ProviderID", p.ID);
                        cmd.Parameters.AddWithValue("@CaseID", caseID);
                        try
                        {
                            p.CaseProviderID = cmd.InsertToIdentity();
                        }
                        catch (Exception e)
                        {
                            Exceptions.Handle(e);
                            throw e;
                        }
                    }
                    ProviderSearchService.UpdateEntry(p.ID.Value);
                    RefreshPatientCache(caseID);
                    StaffingService.PerformCheckByCaseId(caseID);
                }
            }

        }


        public void SaveCaseManagementSummary(SummaryVM model)
        {
            SaveCaseModelSummary(model);
        }


        /******************
            VIEW HELPERS
        ******************/
        public List<Domain.OfficeStaff.OfficeStaff> GetOfficeStaffList()
        {

            // todo handling caching
            return GetOfficeStaffListFromDb();

        }


        public List<ProvidersListItemVM> GetAllProvidersDropdownList()
        {

            using (SqlConnection conn = new SqlConnection(this.connectionString))
            using (SqlCommand cmd = new SqlCommand())
            {

                cmd.Connection = conn;

                cmd.CommandText =
                    "SELECT ID, ProviderType, ProviderFirstName, ProviderLastName FROM dbo.Providers;" +
                    "SELECT ID, ProviderID, ZipCode, IsPrimary FROM dbo.ProviderServiceZipCodes;";


                try
                {

                    DataSet set = cmd.GetDataSet();

                    DataTable providerTable = set.Tables[0];
                    DataTable serviceAreasTable = set.Tables[1];

                    List<ProvidersListItemVM> items = new List<ProvidersListItemVM>();

                    foreach (DataRow r in providerTable.Rows)
                    {

                        var item = new ProvidersListItemVM
                        {
                            ID = r.ToInt("ID"),
                            FirstName = r.ToStringValue("ProviderFirstName"),
                            LastName = r.ToStringValue("ProviderLastName")
                        };
                        if (r.ToIntOrNull("ProviderType") == null)
                        {
                            item.Type = null;
                        }
                        else
                        {
                            item.Type = GetProviderType(r.ToInt("ProviderType"));
                        }

                        // removed when we chopped the service areas and languages from the main list item
                        // whatever uses this object, it should probably be it's own VM

                        //item.ServiceAreas = new List<Domain.Providers.ServiceArea>();

                        //var results = from DataRow areaRow in serviceAreasTable.Rows
                        //              where areaRow.ToInt("ProviderID") == item.ID.Value
                        //              select areaRow;

                        //                        foreach (var a in results) {
                        //                          item.ServiceAreas.Add(new Domain.Providers.ServiceArea() { ID = a.ToInt("ID"), IsPrimary = a.ToBool("IsPrimary"), ZipCode = a.ToStringValue("ZipCode") });
                        //                    }

                        items.Add(item);
                    }

                    return items;

                }
                catch (Exception e)
                {
                    Exceptions.Handle(e);
                    throw new DataException(e.Message, e);
                }


            }


        }


        public List<CaseStatusDescription> GetCaseStatusDescriptionList()
        {

            List<CaseStatusDescription> descs = new List<CaseStatusDescription>
            {
                new CaseStatusDescription() { ID = 0, Description = "Requirements Not Filled" },
                new CaseStatusDescription() { ID = 1, Description = "Good/Active" },
                new CaseStatusDescription() { ID = 2, Description = "History" }
            };

            return descs;

        }


        /******************
            PRIVATE HELPERS
        ******************/
        private List<Domain.OfficeStaff.OfficeStaff> GetOfficeStaffListFromDb()
        {
            using (SqlConnection conn = new SqlConnection(this.connectionString))
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.Connection = conn;
                cmd.CommandText = "SELECT ID, DateCreated, StaffActive, StaffFirstName, StaffLastName, " +
                    "StaffPrimaryPhone, StaffPrimaryEmail, StaffHireDate, StaffTerminatedDate " +
                    "FROM dbo.Staff;";
                try
                {
                    DataTable table = cmd.GetTable();
                    List<Domain.OfficeStaff.OfficeStaff> staffs = new List<Domain.OfficeStaff.OfficeStaff>();
                    foreach (DataRow r in table.Rows)
                    {
                        Domain.OfficeStaff.OfficeStaff staff = new Domain.OfficeStaff.OfficeStaff
                        {
                            ID = r.ToInt("ID"),
                            DateCreated = r.ToDateTime("DateCreated"),
                            Active = r.ToBool("StaffActive"),
                            FirstName = r.ToStringValue("StaffFirstName"),
                            LastName = r.ToStringValue("StaffLastName"),
                            Phone = r.ToStringValue("StaffPrimaryPhone"),
                            Email = r.ToStringValue("StaffPrimaryEmail"),
                            HireDate = r.ToDateTimeOrNull("StaffHireDate"),
                            TerminationDate = r.ToDateTimeOrNull("StaffTerminatedDate")
                        };
                        staffs.Add(staff);
                    }
                    return staffs;
                }
                catch (Exception e)
                {
                    Exceptions.Handle(e);
                    throw new DataException(e.Message, e);
                }
            }
        }


        private void SaveCaseModelSummary(SummaryVM model)
        {
            SaveCaseSummaryCoreModel(model);
            if (model.Providers != null)
            {
                SaveCaseProviders(model.ID.Value, model.Providers);
            }
            if (model.Authorizations != null)
            {
                SaveCaseAuthorizations(model.ID.Value, model.Authorizations);
            }
            RefreshPatientCache(model.ID.Value);
        }


        void SaveCaseAuthorizations(int caseID, List<CaseAuthorization> auths)
        {
            // we will only have an auth code to work off here
            // won't know if it's pre-existing or not, so we have to check everthing
            RemoveAllCaseAuths(caseID);
            foreach (var auth in auths)
            {
                // TODO: handle auths
                int? codeID = GetAuthCodeID(auth.Code);
                int? caseAuthCodeID = null;
                if (codeID == null)
                {
                    codeID = InsertNewAuthCode(auth.Code);
                }
                else
                {
                    caseAuthCodeID = GetCaseAuthCodeID(caseID, codeID.Value);
                }
                if (caseAuthCodeID == null)
                {
                    InsertNewCaseAuthCode(caseID, codeID.Value);
                }
            }
        }


        int? GetCaseAuthCodeID(int caseID, int codeID)
        {
            using (SqlConnection conn = new SqlConnection(this.connectionString))
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.Connection = conn;
                cmd.CommandText = "SELECT ID FROM dbo.CaseAuthCodes WHERE CaseID = @CaseID AND AuthCodeID = @CodeID;";
                cmd.Parameters.AddWithValue("@CaseID", caseID);
                cmd.Parameters.AddWithValue("@CodeID", codeID);
                try
                {
                    return cmd.ExecuteScalarToInt();
                }
                catch
                {
                    return null;
                }
            }
        }


        void InsertNewCaseAuthCode(int caseID, int codeID)
        {
            using (SqlConnection conn = new SqlConnection(this.connectionString))
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.Connection = conn;
                cmd.CommandText = "INSERT INTO dbo.CaseAuthCodes (CaseID, AuthCodeID) VALUES (@CaseID, @CodeID);";
                cmd.Parameters.AddWithValue("@CaseID", caseID);
                cmd.Parameters.AddWithValue("@CodeID", codeID);
                cmd.InsertToIdentity();
            }
        }


        int InsertNewAuthCode(string code)
        {
            using (SqlConnection conn = new SqlConnection(this.connectionString))
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.Connection = conn;
                cmd.CommandText = "INSERT INTO dbo.AuthCodes (CodeCode) VALUES (@Code);";
                cmd.Parameters.AddWithValue("@Code", code);
                return cmd.InsertToIdentity();
            }
        }


        int? GetAuthCodeID(string code)
        {
            using (SqlConnection conn = new SqlConnection(this.connectionString))
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.Connection = conn;
                cmd.CommandText = "SELECT ID FROM dbo.AuthCodes WHERE CodeCode = @Code;";
                cmd.Parameters.AddWithValue("@Code", code);
                try
                {
                    return cmd.ExecuteScalarToInt();
                }
                catch
                {
                    return null;
                }
            }
        }


        void RemoveAllCaseAuths(int caseID)
        {
            using (SqlConnection conn = new SqlConnection(this.connectionString))
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.Connection = conn;
                cmd.CommandText = "DELETE FROM dbo.CaseAuthCodes WHERE CaseID = @CaseID;";
                cmd.Parameters.AddWithValue("@CaseID", caseID);
                cmd.ExecuteNonQueryToInt();
            }
        }


        void SaveCaseProviders(int caseID, List<CaseProvider> providers)
        {
            if (providers.Count == 0)
            {
                return;
            }
            using (SqlConnection conn = new SqlConnection(this.connectionString))
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.Connection = conn;
                foreach (var p in providers)
                {
                    cmd.Parameters.Clear();
                    if (p.CaseProviderID.HasValue)
                    {
                        // update
                        cmd.CommandText = "UPDATE dbo.CaseProviders SET Active = @Active, IsSupervisor = @Super, IsAssessor = @Assessor WHERE ID = @ID;";
                        cmd.Parameters.AddWithValue("@Active", p.Active);
                        cmd.Parameters.AddWithValue("@Super", p.Supervisor);
                        cmd.Parameters.AddWithValue("@Assessor", p.Assessor);
                        cmd.Parameters.AddWithValue("@ID", p.CaseProviderID.Value);
                        cmd.ExecuteNonQuery();
                    }
                    else
                    {
                        // insert
                        cmd.CommandText = "INSERT INTO dbo.CaseProviders (CaseID, ProviderID, Active, IsSupervisor, IsAssessor) VALUES (@CaseID, @ProviderID, @Active, @Super, @Assessor);";
                        cmd.Parameters.AddWithValue("@CaseID", caseID);
                        cmd.Parameters.AddWithValue("@Super", p.Supervisor);
                        cmd.Parameters.AddWithValue("@Assessor", p.Assessor);
                        cmd.Parameters.AddWithValue("@ProviderID", p.ID.Value);
                        cmd.Parameters.AddWithValue("@Active", p.Active);
                        p.CaseProviderID = cmd.InsertToIdentity();
                    }
                }
            }
        }


        void SaveCaseSummaryCoreModel(SummaryVM model)
        {
            //if (model.Patient == null || !model.Patient.ID.HasValue) {
            //    throw new ArgumentNullException("CaseSummaryViewModel.Patient");
            //}
            using (SqlConnection conn = new SqlConnection(this.connectionString))
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.Connection = conn;
                if (model.ID.HasValue)
                {
                    string sql = "";
                    sql += "UPDATE dbo.Cases SET CaseStatus = @, CaseStatusNotes = @, CaseStartDate = @, CaseAssignedStaffID = @, ";
                    sql += "  CaseRequiredHoursNotes = @, CaseRequiredServicesNotes = @, CaseHasPrescription = @, CaseHasAssessment = @, ";
                    sql += "  CaseHasIntake = @, CaseStatusReason = @, DefaultServiceLocationID = @, FunctioningLevelID = @ ";
                    sql += "WHERE ID = @ID";
                    sql += ";";
                    var pi = new List<ParameterInfo>
                    {
                        new ParameterInfo() { Value = model.Status, Nullable = false },
                        new ParameterInfo() { Value = model.StatusNotes, Nullable = true },
                        new ParameterInfo() { Value = model.StartDate, Nullable = true },
                        new ParameterInfo() { Value = model.AssignedStaff?.ID, Nullable = true },  // assign manually (index 3)
                        new ParameterInfo() { Value = model.RequiredHoursNotes, Nullable = true },
                        new ParameterInfo() { Value = model.RequiredServicesNotes, Nullable = true },
                        new ParameterInfo() { Value = model.HasPrescription, Nullable = false },
                        new ParameterInfo() { Value = model.HasAssessment, Nullable = false },
                        new ParameterInfo() { Value = model.HasIntake, Nullable = false },
                        new ParameterInfo() { Value = model.StatusReason, Nullable = false },
                        new ParameterInfo() { Value = model.DefaultServiceLocation?.ID, Nullable = true },
                        new ParameterInfo() { Value = model.FunctioningLevel?.ID, Nullable = true }
                    };
                    //if (model.AssignedStaff != null)
                    //{
                    //    pi[3].Value = model.AssignedStaff.ID;
                    //}
                    cmd.CommandText = sql;
                    cmd.AddParameters(pi.ToArray());
                    cmd.Parameters.AddWithValue("@ID", model.ID.Value);
                    try
                    {
                        cmd.ExecuteNonQueryToInt();
                    }
                    catch (Exception e)
                    {
                        Exceptions.Handle(e);
                        throw e;
                    }
                    return;
                }
                else
                {
                    string sql = "";
                    sql += "INSERT INTO dbo.Cases (PatientID, CaseStatus, CaseStatusNotes, CaseStartDate, CaseAssignedStaffID, ";
                    sql += "  CaseRequiredHoursNotes, CaseRequiredServicesNotes, CaseHasPrescription, CaseHasAssessment, ";
                    sql += "  CaseHasIntake, DefaultServiceLocationID) VALUES (@[ALLPARAMS]);";

                    ParameterInfo[] pi = new ParameterInfo[11]
                    {
                        new ParameterInfo() { Value = model.Patient.ID, Nullable = false },
                        new ParameterInfo() { Value = model.Status, Nullable = false },
                        new ParameterInfo() { Value = model.StatusNotes, Nullable = true },
                        new ParameterInfo() { Value = model.StartDate, Nullable = true },
                        new ParameterInfo() { Value = null, Nullable = true },  // assign manually (index 3)
                        new ParameterInfo() { Value = model.RequiredHoursNotes, Nullable = true },
                        new ParameterInfo() { Value = model.RequiredServicesNotes, Nullable = true },
                        new ParameterInfo() { Value = model.HasPrescription, Nullable = false },
                        new ParameterInfo() { Value = model.HasAssessment, Nullable = false },
                        new ParameterInfo() { Value = model.HasIntake, Nullable = false },
                        new ParameterInfo() { Value = model.DefaultServiceLocation?.ID, Nullable = true }
                    };
                    if (model.AssignedStaff != null)
                    {
                        pi[4].Value = model.AssignedStaff.ID;
                    }
                    cmd.CommandText = sql;
                    cmd.AddParameters(pi);
                    model.ID = cmd.InsertToIdentity();
                    return;
                }
            }
        }


        private SummaryVM GetCaseSummary(int caseID)
        {
            PatientRepository.RecalculateCaseStatus(caseID);
            var model = new SummaryVM();
            using (SqlConnection conn = new SqlConnection(this.connectionString))
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.Connection = conn;
                string caseSql = "";
                caseSql += "SELECT ID, DateCreated, PatientID, CaseStatus, CaseStatusNotes, CaseStartDate, ";
                caseSql += "  CaseAssignedStaffID, CaseRequiredHoursNotes, CaseRequiredServicesNotes, ";
                caseSql += "  CaseHasPrescription, CaseHasAssessment, ";
                caseSql += "  CaseHasIntake, CaseStatusReason, DefaultServiceLocationID, FunctioningLevelID ";
                caseSql += "FROM dbo.Cases ";
                caseSql += "WHERE ID = @CaseID ";
                caseSql += ";";

                string patientSql = "";
                patientSql += "SELECT p.ID, p.PatientFirstName, p.PatientLastName, p.PatientDateOfBirth, p.PatientGender, ";
                patientSql += "  p.PatientGuardianFirstName, p.PatientGuardianLastName, p.PatientGuardianRelationship, p.HighRisk ";
                patientSql += "FROM dbo.Patients AS p ";
                patientSql += "INNER JOIN dbo.Cases AS c ON c.PatientID = p.ID ";
                patientSql += "WHERE c.ID = @CaseID ";
                patientSql += ";";

                string providersSql = "";
                providersSql += "SELECT p.ID, p.ProviderType, p.ProviderFirstName, p.ProviderLastName, cp.Active, cp.ID AS CaseProviderID, cp.IsAssessor, cp.IsSupervisor, ";
                providersSql += "  pt.ID AS ProviderTypeID, pt.ProviderTypeCode, pt.ProviderTypeName, pt.ProviderTypeCanSuperviseCase, pt.ProviderTypeIsOutsourced ";
                providersSql += "FROM dbo.Providers AS p ";
                providersSql += "INNER JOIN dbo.CaseProviders AS cp ON cp.ProviderID = p.ID ";
                providersSql += "LEFT JOIN dbo.ProviderTypes AS pt ON pt.ID = p.ProviderType ";
                providersSql += "WHERE cp.CaseID = @CaseID ";
                providersSql += ";";

                string assignedStaffSql = "";
                assignedStaffSql += "SELECT s.ID, s.DateCreated, s.StaffActive, s.StaffFirstName, s.StaffLastName ";
                assignedStaffSql += "FROM dbo.Staff AS s ";
                assignedStaffSql += "INNER JOIN dbo.Cases AS c ON c.CaseAssignedStaffID = s.ID ";
                assignedStaffSql += "WHERE c.ID = @CaseID ";
                assignedStaffSql += ";";

                cmd.Parameters.AddWithValue("@CaseID", caseID);
                cmd.CommandText = caseSql + patientSql + providersSql + assignedStaffSql;
                try
                {
                    var set = cmd.GetDataSet(new string[] { "Case", "Patient", "Providers", "AssignedStaff" });
                    Domain.OfficeStaff.OfficeStaff assignedStaff = null;
                    if (set.Tables["AssignedStaff"].Rows.Count > 0)
                    {
                        assignedStaff = new Domain.OfficeStaff.OfficeStaff();
                        var r = set.Tables["AssignedStaff"].Rows[0];
                        assignedStaff.ID = r.ToInt("ID");
                        assignedStaff.DateCreated = r.ToDateTime("DateCreated");
                        assignedStaff.Active = r.ToBool("StaffActive");
                        assignedStaff.FirstName = r.ToStringValue("StaffFirstName");
                        assignedStaff.LastName = r.ToStringValue("StaffLastName");
                    }

                    List<CaseProvider> providers = new List<CaseProvider>();
                    if (set.Tables["Providers"].Rows.Count > 0)
                    {
                        foreach (DataRow r in set.Tables["Providers"].Rows)
                        {
                            var p = new CaseProvider
                            {
                                ID = r.ToInt("ID"),
                                CaseProviderID = r.ToInt("CaseProviderID"),
                                Active = r.ToBool("Active"),
                                FirstName = r.ToStringValue("ProviderFirstName"),
                                LastName = r.ToStringValue("ProviderLastName"),
                                Assessor = r.ToBool("IsAssessor"),
                                Supervisor = r.ToBool("IsSupervisor"),
                                Type = new Domain.Providers.ProviderType()
                            };
                            if (r.ToIntOrNull("ProviderTypeID") != null)
                            {
                                p.Type.ID = r.ToInt("ProviderTypeID");
                                p.Type.Name = r.ToStringValue("ProviderTypeName");
                                p.Type.Code = r.ToStringValue("ProviderTypeCode");
                                p.Type.CanSuperviseCase = r.ToBool("ProviderTypeCanSuperviseCase");
                                p.Type.IsOutsourced = r.ToBool("ProviderTypeIsOutsourced");
                            }
                            providers.Add(p);
                        }
                    }

                    Domain.Patients.Patient patient = new Domain.Patients.Patient();
                    if (set.Tables["Patient"].Rows.Count > 0)
                    {
                        var r = set.Tables["Patient"].Rows[0];
                        patient.ID = r.ToInt("ID");
                        patient.FirstName = r.ToStringValue("PatientFirstName");
                        patient.LastName = r.ToStringValue("PatientLastName");
                        patient.Gender = r.ToIntOrNull("PatientGender") != null ? (Domain.Gender?)Enum.Parse(typeof(Domain.Gender), r.ToStringValue("PatientGender")) : null;
                        patient.DateOfBirth = r.ToDateTimeOrNull("PatientDateOfBirth");
                        patient.GuardianFirstName = r.ToStringValue("PatientGuardianFirstName");
                        patient.GuardianLastName = r.ToStringValue("PatientGuardianLastName");
                        patient.GuardianRelationship = r.ToStringValue("PatientGuardianRelationship");
                        patient.HighRisk = r.ToBool("HighRisk");
                    }

                    var row = set.Tables["Case"].Rows[0];
                    model.ID = caseID;
                    model.Status = (CaseStatus)row.ToInt("CaseStatus");
                    model.StatusNotes = row.ToStringValue("CaseStatusNotes");
                    model.StartDate = row.ToDateTimeOrNull("CaseStartDate");
                    model.RequiredHoursNotes = row.ToStringValue("CaseRequiredHoursNotes");
                    model.RequiredServicesNotes = row.ToStringValue("CaseRequiredServicesNotes");
                    model.HasPrescription = row.ToBool("CaseHasPrescription");
                    model.HasAssessment = row.ToBool("CaseHasAssessment");
                    model.HasIntake = row.ToBool("CaseHasIntake");
                    model.StatusReason = (CaseStatusReason)row.ToInt("CaseStatusReason");
                    model.DefaultServiceLocationID = row.ToIntOrNull("DefaultServiceLocationID");
                    model.FunctioningLevelID = row.ToIntOrNull("FunctioningLevelID");
                    if (model.DefaultServiceLocationID.HasValue)
                    {
                        model.DefaultServiceLocation = new Data.Services.ServicesService().GetActiveServiceLocations().Where(x => x.ID == model.DefaultServiceLocationID.Value).FirstOrDefault();
                    }
                    if (model.FunctioningLevelID.HasValue)
                    {
                        model.FunctioningLevel = new Data.Services.ServicesService().GetFunctioningLevels().Where(x => x.ID == model.FunctioningLevelID.Value).FirstOrDefault();
                    }
                    model.Patient = patient;
                    model.AssignedStaff = assignedStaff;
                    model.Providers = providers;
                    return model;
                }
                catch (Exception e)
                {
                    Exceptions.Handle(e);
                    throw e;
                }
            }
        }


        private ProvidersVM GetCaseManagementProvider(int caseID)
        {
            var model = new ProvidersVM();
            using (SqlConnection conn = new SqlConnection(this.connectionString))
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.Connection = conn;
                string caseSql = "";
                caseSql += "SELECT ID, DateCreated, PatientID, CaseStatus, CaseStatusNotes, CaseStartDate, ";
                caseSql += "  CaseAssignedStaffID, CaseRequiredHoursNotes, CaseRequiredServicesNotes, ";
                caseSql += "  CaseHasPrescription, CaseHasAssessment, ";
                caseSql += "  CaseHasIntake ";
                caseSql += "FROM dbo.Cases ";
                caseSql += "WHERE ID = @CaseID ";
                caseSql += ";";

                string patientSql = "";
                patientSql += "SELECT p.ID, p.PatientFirstName, p.PatientLastName, p.PatientDateOfBirth ";
                patientSql += "FROM dbo.Patients AS p ";
                patientSql += "INNER JOIN dbo.Cases AS c ON c.PatientID = p.ID ";
                patientSql += "WHERE c.ID = @CaseID ";
                patientSql += ";";

                string providersSql = "";
                providersSql += "SELECT p.ID, p.ProviderType, p.ProviderFirstName, p.ProviderLastName, p.ProviderPrimaryEmail, p.ProviderPrimaryPhone, cp.Active, cp.ID AS CaseProviderID, ";
                providersSql += "  pt.ID AS ProviderTypeID, pt.ProviderTypeCode, pt.ProviderTypeName, pt.ProviderTypeCanSuperviseCase, pt.ProviderTypeIsOutsourced, ";
                providersSql += "  cp.IsSupervisor, cp.IsAssessor ";
                providersSql += "FROM dbo.Providers AS p ";
                providersSql += "INNER JOIN dbo.CaseProviders AS cp ON cp.ProviderID = p.ID ";
                providersSql += "LEFT JOIN dbo.ProviderTypes AS pt ON pt.ID = p.ProviderType ";
                providersSql += "WHERE cp.CaseID = @CaseID ";
                providersSql += ";";

                string assignedStaffSql = "";
                assignedStaffSql += "SELECT s.ID, s.DateCreated, s.StaffActive, s.StaffFirstName, s.StaffLastName ";
                assignedStaffSql += "FROM dbo.Staff AS s ";
                assignedStaffSql += "INNER JOIN dbo.Cases AS c ON c.CaseAssignedStaffID = s.ID ";
                assignedStaffSql += "WHERE c.ID = @CaseID ";
                assignedStaffSql += ";";
                cmd.Parameters.AddWithValue("@CaseID", caseID);
                cmd.CommandText = caseSql + patientSql + providersSql + assignedStaffSql;
                try
                {
                    var set = cmd.GetDataSet(new string[] { "Case", "Patient", "Providers", "AssignedStaff" });
                    Domain.OfficeStaff.OfficeStaff assignedStaff = null;
                    if (set.Tables["AssignedStaff"].Rows.Count > 0)
                    {
                        assignedStaff = new Domain.OfficeStaff.OfficeStaff();
                        var r = set.Tables["AssignedStaff"].Rows[0];
                        assignedStaff.ID = r.ToInt("ID");
                        assignedStaff.DateCreated = r.ToDateTime("DateCreated");
                        assignedStaff.Active = r.ToBool("StaffActive");
                        assignedStaff.FirstName = r.ToStringValue("StaffFirstName");
                        assignedStaff.LastName = r.ToStringValue("StaffLastName");
                    }
                    List<CaseProvider> providers = new List<CaseProvider>();
                    if (set.Tables["Providers"].Rows.Count > 0)
                    {
                        foreach (DataRow r in set.Tables["Providers"].Rows)
                        {
                            var p = new CaseProvider
                            {
                                ID = r.ToInt("ID"),
                                CaseProviderID = r.ToInt("CaseProviderID"),
                                Active = r.ToBool("Active"),
                                FirstName = r.ToStringValue("ProviderFirstName"),
                                LastName = r.ToStringValue("ProviderLastName"),
                                Email = r.ToStringValue("ProviderPrimaryEmail"),
                                Phone = r.ToStringValue("ProviderPrimaryPhone"),
                                Type = new Domain.Providers.ProviderType()
                            };
                            if (r.ToIntOrNull("ProviderTypeID") != null)
                            {
                                p.Type.ID = r.ToInt("ProviderTypeID");
                                p.Type.Name = r.ToStringValue("ProviderTypeName");
                                p.Type.Code = r.ToStringValue("ProviderTypeCode");
                                p.Type.CanSuperviseCase = r.ToBool("ProviderTypeCanSuperviseCase");
                                p.Type.IsOutsourced = r.ToBool("ProviderTypeIsOutsourced");
                                p.Supervisor = r.ToBool("IsSupervisor");
                                p.Assessor = r.ToBool("IsAssessor");
                            }
                            providers.Add(p);
                        }
                    }

                    Domain.Patients.Patient patient = new Domain.Patients.Patient();
                    if (set.Tables["Patient"].Rows.Count > 0)
                    {
                        var r = set.Tables["Patient"].Rows[0];
                        patient.ID = r.ToInt("ID");
                        patient.FirstName = r.ToStringValue("PatientFirstName");
                        patient.LastName = r.ToStringValue("PatientLastName");
                        patient.DateOfBirth = r.ToDateTimeOrNull("PatientDateOfBirth");
                    }
                    var row = set.Tables["Case"].Rows[0];
                    model.ID = caseID;
                    model.Status = (CaseStatus)row.ToInt("CaseStatus");
                    model.StatusNotes = row.ToStringValue("CaseStatusNotes");
                    model.StartDate = row.ToDateTimeOrNull("CaseStartDate");
                    model.RequiredHoursNotes = row.ToStringValue("CaseRequiredHoursNotes");
                    model.RequiredServicesNotes = row.ToStringValue("CaseRequiredServicesNotes");
                    model.HasPrescription = row.ToBool("CaseHasPrescription");
                    model.HasAssessment = row.ToBool("CaseHasAssessment");
                    model.HasIntake = row.ToBool("CaseHasIntake");
                    model.Patient = patient;
                    model.AssignedStaff = assignedStaff;
                    model.Providers = providers;
                    return model;
                }
                catch (Exception e)
                {
                    Exceptions.Handle(e);
                    throw e;
                }
            }
        }


        private ProviderTypeVM GetProviderType(int id)
        {
            using (SqlConnection conn = new SqlConnection(this.connectionString))
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.Connection = conn;
                cmd.CommandText =
                    "SELECT ID, DateCreated, ProviderTypeCode, ProviderTypeName, " +
                    "  ProviderTypeIsOutsourced, ProviderTypeCanSuperviseCase " +
                    "FROM dbo.ProviderTypes " +
                    "WHERE ID = @ID;";
                cmd.Parameters.AddWithValue("@ID", id);
                try
                {
                    DataTable table = cmd.GetTable();
                    if (table.Rows.Count == 0)
                    {
                        return null;
                    }
                    DataRow r = table.Rows[0];
                    var t = new ProviderTypeVM
                    {
                        ID = r.ToInt("ID"),
                        DateCreated = r.ToDateTime("DateCreated"),
                        Code = r.ToStringValue("ProviderTypeCode"),
                        Name = r.ToStringValue("ProviderTypeName"),
                        IsOutsourced = r.ToBool("ProviderTypeIsOutsourced"),
                        CanSuperviseCase = r.ToBool("ProviderTypeCanSuperviseCase")
                    };
                    return t;
                }
                catch (Exception e)
                {
                    Exceptions.Handle(e);
                    throw new DataException(e.Message, e);
                }
            }
        }


        private int GetCaseProviderID(int caseID, int providerID)
        {
            using (SqlConnection conn = new SqlConnection(this.connectionString))
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.Connection = conn;
                cmd.CommandText = "SELECT TOP 1 ID FROM dbo.CaseProviders WHERE CaseID = @CaseID AND ProviderID = @ProviderID;";
                cmd.Parameters.AddWithValue("@CaseID", caseID);
                cmd.Parameters.AddWithValue("@ProviderID", providerID);
                try
                {
                    return cmd.ExecuteScalarToInt();
                }
                catch (Exception e)
                {
                    Exceptions.Handle(e);
                    throw e;
                }
            }
        }


        public IEnumerable<Domain2.Cases.FunctioningLevel> GetFunctioningLevelList()
        {
            return Context.FunctioningLevels.OrderBy(m => m.Name).ToList();
        }


        private void RefreshPatientCache(int caseID)
        {
            var @case = Context.Cases.SingleOrDefault(m => m.ID == caseID);
            if (@case != null)
            {
                PatientSearchService.UpdateEntry(@case.PatientID);
            }
        }


        private readonly string connectionString;
        private readonly CoreContext Context;
        private readonly PatientRepository PatientRepository;
        private readonly ProviderSearchService ProviderSearchService;
        private readonly PatientSearchService PatientSearchService;
        private readonly IStaffingService StaffingService;

        public CaseRepository()
        {
            connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["CoreConnection"].ConnectionString;
            Context = AppService.Current.DataContextV2;
            PatientRepository = new PatientRepository();
            ProviderSearchService = new ProviderSearchService(Context);
            PatientSearchService = new PatientSearchService(Context);
            StaffingService = new StaffingService(Context);
        }

    }
}