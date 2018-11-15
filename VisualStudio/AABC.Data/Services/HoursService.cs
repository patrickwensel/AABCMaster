using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace AABC.Data.Services
{
    public class HoursService
    {



        



        public List<Models.Sprocs.PeriodHoursMatrixByCase> PeriodHoursMatrix(int caseID, DateTime startDate, DateTime endDate) {

            var context = new Models.CoreEntityModel();

            return context.Database.SqlQuery<Models.Sprocs.PeriodHoursMatrixByCase>(
                "dbo.GetPeriodHoursMatrixByCase @CaseID, @StartDate, @EndDate",
                new SqlParameter("@CaseID", caseID),
                new SqlParameter("@StartDate", startDate),
                new SqlParameter("@EndDate", endDate)
                ).ToList();

        }



        public void SetHasDataTrue(int[] hourIDs) {

            using (SqlConnection conn = new SqlConnection(this.connectionString))
                using (SqlCommand cmd = new SqlCommand()) {

                cmd.Connection = conn;
                cmd.CommandText = "UPDATE dbo.CaseAuthHours SET HoursHasCatalystData = 1 WHERE ID = @ID;";
                cmd.Parameters.Add("@ID", SqlDbType.Int);
                conn.Open();
                foreach(int id in hourIDs) {
                    cmd.Parameters["@ID"].Value = id;
                    cmd.ExecuteNonQuery();
                }
                conn.Close();
            }
            
        }


        public List<AABC.Domain.Providers.PayrollHours> GetPayrollHoursByPeriod(DateTime periodFirstDayOfMonth, int filter) {

            // filter has to match with AABC.Web.App.Providers.Models.PayrollFilter
            // due to this being an old structure, this isn't the ideal way to handle it
            // (this data project should be refactored so it doesn't need the filter, but
            //  it's a big project, so we'll cheat for now)
            //  int filter:
            //    0: non
            //    1: NY only
            //    2: non-NY
            
            var context = new Models.CoreEntityModel();

            var data = context.Database.SqlQuery<PayrollHoursByPeriodSprocDTO>(
                "webreports.PayablesByPeriod @FirstDayOfMonth, @StateFilter",
                new System.Data.SqlClient.SqlParameter("@FirstDayOfMonth", periodFirstDayOfMonth),
                new System.Data.SqlClient.SqlParameter("@StateFilter", filter)
                ).ToList();

            var items = new List<Domain.Providers.PayrollHours>();

            foreach (var dataItem in data) {
                var item = new Domain.Providers.PayrollHours();
                item.ID = dataItem.ID;
                item.PayrollID = dataItem.PayrollID;
                item.FirstName = dataItem.ProviderFirstName;
                item.LastName = dataItem.ProviderLastName;
                item.TotalHours = (double)dataItem.TotalPayable;
                item.EntriesMissingCatalystData = dataItem.EntriesMissingCatalystData;
                items.Add(item);
            }

            return items;

        }

        private class PayrollHoursByPeriodSprocDTO
        {
            public int ID { get; set; }
            public int PayrollID { get; set; }
            public string ProviderFirstName { get; set; }
            public string ProviderLastName { get; set; }
            public decimal TotalPayable { get; set; }
            public int EntriesMissingCatalystData { get; set; }
        }



        string connectionString;

        public HoursService() {
            this.connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["CoreConnection"].ConnectionString;
        }
        public HoursService(string connectionString) {
            this.connectionString = connectionString;
        }

        public List<Domain.Hours.HoursDownload> GetHoursForDownload(int providerID, int caseID, int month, int year)
        {
            var context = new Models.CoreEntityModel();
            var entities = context.Database.SqlQuery<Domain.Hours.HoursDownload>(
                "[dbo].GetHoursForDownload @ProviderID, @CaseID, @Month, @Year",
                new SqlParameter("@ProviderID", providerID),
                new SqlParameter("@CaseID", caseID),
                new SqlParameter("@Month", month),
                new SqlParameter("@Year", year)
                ).ToList();

            return entities;
        }

        public List<Domain.Cases.CaseAuthorizationHours> GetHoursByPeriod(DateTime period, bool includeNonFinalized) {


            DateTime startDate = period;
            DateTime endDate = period.AddMonths(1).AddMilliseconds(-1);

            var context = new Models.CoreEntityModel();

            int minStatus = includeNonFinalized ? 0 : 2;

            var entities = context.Database.SqlQuery<Models.Sprocs.HoursDetailedByPeriod>(
                "GetDetailedHoursByPeriod @StartDate, @EndDate, @MinStatus",
                new SqlParameter("@StartDate", startDate),
                new SqlParameter("@EndDate", endDate),
                new SqlParameter("@MinStatus", minStatus)
                ).ToList();

            var items = new List<Domain.Cases.CaseAuthorizationHours>();

            foreach (var h in entities) {

                var cah = new Domain.Cases.CaseAuthorizationHours();

                cah.Authorization = new Domain.Cases.CaseAuthorization() { ID = h.CaseAuthID };
                cah.CaseID = h.CaseID;
                cah.ProviderID = h.CaseProviderID;
                cah.Provider = new Domain.Cases.CaseProvider() { ID = h.CaseProviderID };
                cah.DateCreated = h.DateCreated;
                cah.BillableHours = (double?)h.HoursBillable;
                cah.BillingRef= h.HoursBillingRef;
                cah.CorrelationID = h.HoursCorrelationID;
                cah.Date = h.HoursDate;
                cah.HasCatalystData= h.HoursHasCatalystData;
                cah.InternalNotes = h.HoursInternalNotes;
                cah.Notes = h.HoursNotes;
                cah.HasExtendedNotes = h.HasExtendedNotes == 1;
                cah.PayableHours= (double?)h.HoursPayable;
                cah.PayableRef= h.HoursPayableRef;
                cah.Service = new Domain.Cases.Service() { ID = h.HoursServiceID };
                cah.SSGParentID = h.HoursSSGParentID;
                cah.Status = (Domain.Cases.AuthorizationHoursStatus)h.HoursStatus;
                cah.TimeIn = h.HoursDate + h.HoursTimeIn;
                cah.TimeOut = h.HoursDate + h.HoursTimeOut;
                cah.HoursTotal = (double)h.HoursTotal;
                cah.WatchEnabled = h.HoursWatchEnabled;
                cah.WatchNote = h.HoursWatchNote;
                cah.ID = h.ID;
                cah.IsPayrollOrBillingAdjustment = h.IsPayrollOrBillingAdjustment;
                cah.Case = new Domain.Cases.Case() { ID = h.CaseID, Patient = new Domain.Patients.Patient() { FirstName = h.PatientName } };
                cah.Provider.FirstName = h.ProviderName;
                cah.ServiceLocationID = h.ServiceLocationID;
                cah.Service.Code = h.ServiceCode;
                cah.Authorization = new Domain.Cases.CaseAuthorization() { CaseAuthorizationID = h.CaseAuthID, Code = h.AuthCode };

                items.Add(cah);

            }

            return items;
            
        }
    }

}
