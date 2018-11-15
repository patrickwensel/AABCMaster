using Dymeng.Framework.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace AABC.Web.Repos
{
    public class HoursRepo
    {



        public void FillScrubSummaryProviderInfo(App.Hours.Models.ScrubOverviewItemSummaryVM summary, int caseID, DateTime periodStartDate) {

            DateTime periodEndDate = periodStartDate.AddMonths(1).AddDays(-1);

            var q = new Data.Models.CoreEntityModel().Database.SqlQuery<Data.Models.Sprocs.HoursScrubSummaryProvidersLists>(
                "dbo.GetHoursScrubSummaryProviders @CaseID, @StartDate, @EndDate",
                new SqlParameter("@CaseID", caseID),
                new SqlParameter("@StartDate", periodStartDate),
                new SqlParameter("@EndDate", periodEndDate)
                );

            var entities = q.ToList();
            
            foreach (var entity in entities) {

                switch (entity.ReturnTypeID) {
                    case 0:
                        summary.ActiveProviders.Add(new App.Hours.Models.ScrubOverviewItemSummaryProvider()
                        {
                            ID = entity.ID,
                            FirstName = entity.ProviderFirstName,
                            LastName = entity.ProviderLastName
                        });
                        break;
                    case 1:
                        summary.ProvidersWithHours.Add(new App.Hours.Models.ScrubOverviewItemSummaryProvider()
                        {
                            ID = entity.ID,
                            FirstName = entity.ProviderFirstName,
                            LastName = entity.ProviderLastName
                        });
                        break;
                    case 2:
                        summary.ProvidersWithoutHours.Add(new App.Hours.Models.ScrubOverviewItemSummaryProvider()
                        {
                            ID = entity.ID,
                            FirstName = entity.ProviderFirstName,
                            LastName = entity.ProviderLastName
                        });
                        break;

                    case 3:
                        summary.ProvidersFinalized.Add(new App.Hours.Models.ScrubOverviewItemSummaryProvider()
                        {
                            ID = entity.ID,
                            FirstName = entity.ProviderFirstName,
                            LastName = entity.ProviderLastName
                        });
                        break;
                    case 4:
                        summary.ProvidersNotFinalized.Add(new App.Hours.Models.ScrubOverviewItemSummaryProvider()
                        {
                            ID = entity.ID,
                            FirstName = entity.ProviderFirstName,
                            LastName = entity.ProviderLastName
                        });
                        break;
                    default:
                        throw new ArgumentOutOfRangeException("ReturnTypeID not registered");
                }

            }
            

        }

        internal string GetPatientName(int caseID) {

            using (SqlConnection conn = new SqlConnection(this.connectionString))
                using (SqlCommand cmd = new SqlCommand()) {

                cmd.Connection = conn;

                cmd.CommandText = "SELECT p.PatientFirstName, p.PatientLastName FROM dbo.Patients AS p INNER JOIN dbo.Cases AS c ON c.PatientID = p.ID WHERE c.ID = @CaseID;";
                cmd.Parameters.AddWithValue("@CaseID", caseID);

                var row = cmd.GetRow();

                return row.ToStringValue(0) + " " + row.ToStringValue(1);

            }

        }

        public void FinalizeAllPerCaseAndPeriod(int caseID, DateTime periodStartDate) {

            using (SqlConnection conn = new SqlConnection(this.connectionString))
                using (SqlCommand cmd = new SqlCommand()) {

                cmd.Connection = conn;
                cmd.Connection.Open();

                SqlTransaction transaction = conn.BeginTransaction();                
                cmd.Transaction = transaction;

                try {
                    
                    DateTime startDate = periodStartDate;
                    DateTime endDate = startDate.AddMonths(1).AddDays(-1);

                    cmd.Parameters.AddWithValue("@CaseID", caseID);
                    cmd.Parameters.AddWithValue("@Start", startDate);
                    cmd.Parameters.AddWithValue("@End", endDate);

                    cmd.CommandText =
                        "UPDATE cah SET cah.CaseID = cac.CaseID " +
                        "FROM dbo.CaseAuthHours AS cah LEFT JOIN dbo.CaseAuthCodes AS cac ON cac.ID = cah.CaseAuthID " +
                        "WHERE cah.CaseID IS NULL AND cah.HoursStatus = 2 AND cah.HoursDate >= @Start AND cah.HoursDate <= @End AND cac.CaseID = @CaseID;";
                    
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = 
                        "UPDATE dbo.CaseAuthHours SET HoursStatus = 3 " + 
                        "WHERE CaseID = @CaseID AND HoursDate >= @Start AND HoursDate <= @End AND HoursStatus = 2;";

                    cmd.ExecuteNonQuery();
                    
                    transaction.Commit();

                    cmd.Connection.Close();

                } catch (Exception) {
                    try {
                        transaction.Rollback();
                    } catch (Exception) {

                    }
                }
                
            }

        }

        internal List<App.Hours.Models.UnfinalizedProviderExportItemVM> GetUnfinalizedProviderExportItems(DateTime period) {

            using (SqlConnection conn = new SqlConnection(this.connectionString))
                using (SqlCommand cmd = new SqlCommand()) {

                cmd.Connection = conn;

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "dbo.GetUnfinalizedProviders";
                cmd.Parameters.AddWithValue("@StartDate", period);
                cmd.Parameters.AddWithValue("@EndDate", period.AddMonths(1));

                var table = cmd.GetTable();

                var items = new List<App.Hours.Models.UnfinalizedProviderExportItemVM>();

                foreach (DataRow r in table.Rows) {

                    items.Add(new App.Hours.Models.UnfinalizedProviderExportItemVM()
                    {
                        FirstName = r.ToStringValue("ProviderFirstName"),
                        LastName = r.ToStringValue("ProviderLastName"),
                        Email = r.ToStringValue("ProviderPrimaryEmail"),
                        HoursCount = r.ToInt("HoursCount"),
                        HasFinalization = r.ToStringValue("HasFinalization")
                    });                   
                    
                }

                return items;

            }

        }

        public List<App.Hours.Models.MHBExportItemVM> GetMHBExportItems(DateTime firstDayOfMonthOfPeriod, string billingRef) {

            using (SqlConnection conn = new SqlConnection(this.connectionString))
                using (SqlCommand cmd = new SqlCommand()) {

                cmd.Connection = conn;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "GetMBHMonthlyBillingExport";
                cmd.Parameters.AddWithValue("@FirstDayOfMonth", firstDayOfMonthOfPeriod);
                cmd.Parameters.AddWithNullableValue("@BillingRef", billingRef);

                DataTable table = cmd.GetTable();

                var items = new List<App.Hours.Models.MHBExportItemVM>();


                foreach (DataRow r in table.Rows) {
                    var item = new App.Hours.Models.MHBExportItemVM();

                    item.HoursID = r.ToInt("HoursID");
                    item.PatientFN = r.ToStringValue("PatientFN");
                    item.PatientLN = r.ToStringValue("PatientLN");
                    item.ProviderFN = r.ToStringValue("ProviderFN");
                    item.ProviderLN = r.ToStringValue("ProviderLN");
                    item.ProviderType = r.ToStringValue("ProviderType");

                    item.CaseID = r.ToInt("CaseID");
                    item.DateOfService = r.ToDateTime("DateOfService");
                    item.EndTime = r.ToDateTime("EndTime");
                    item.IsBCBATimesheet = r.ToBool("IsBCBATimesheet");
                    item.PatientID = r.ToInt("PatientID");
                    item.PlaceOfService = r.ToStringValue("PlaceOfService");
                    item.PlaceOfServiceID = r.ToIntOrNull("PlaceOfServiceID");
                    item.ProviderID = r.ToInt("ProviderID");
                    item.ServiceCode = r.ToStringValue("ServiceCode");
                    item.StartTime = r.ToDateTime("StartTime");
                    item.TotalTime = r.ToDouble("TotalTime");
                    item.PlaceOfService = r.ToStringValue("PlaceOfService");
                    item.PlaceOfServiceID = r.ToIntOrNull("PlaceOfServiceID");
                    item.InsuranceAuthorizedBCBA = r.ToIntOrNull("InsurancedAuthorizedProviderID");

                    if (!item.PlaceOfServiceID.HasValue) {
                        item.PlaceOfServiceID = 1;
                        item.PlaceOfService = "Home";
                    }

                    items.Add(item);
                }

                
                items.Where(x => x.IsBCBATimesheet == false).ToList().ForEach((x) => {
                    x.SupervisingBCBAID = GetBCBASupervisor(x.CaseID, x.DateOfService);
                });
                
                return items;
            }

        }


        public int? GetBCBASupervisor(int caseID, DateTime hoursDate) {

            using (SqlConnection conn = new SqlConnection(this.connectionString))
                using (SqlCommand cmd = new SqlCommand()) {

                cmd.Connection = conn;
                cmd.CommandText =
                    "SELECT TOP 1 ProviderID " +
                    "FROM dbo.CaseProviders " +
                    "WHERE CaseID = @CaseID " +
                        "AND Active = 1 " +
                        "AND IsSupervisor = 1 " +
                        "AND (ActiveEndDate IS NULL OR ActiveEndDate >= @RefDate) " +
                        "AND (ActiveStartDate IS NULL OR ActiveStartDate <= @RefDate) " +
                    ";";


                cmd.Parameters.AddWithValue("@CaseID", caseID);
                cmd.Parameters.AddWithValue("@RefDate", hoursDate);

                var r = cmd.GetRowOrNull();
                if (r == null) {
                    return null;
                } else {
                    return r.ToIntOrNull("ProviderID");
                }

            }
        }

        public List<Models.Providers.PayrollGridItemVM> GetPayablesByPeriod(DateTime firstDayOfMonthOfPeriod, App.Providers.Models.PayrollFilter filter) {
            
            var domainList = new Data.Services.HoursService().GetPayrollHoursByPeriod(firstDayOfMonthOfPeriod, (int)filter);

            var list = new List<Models.Providers.PayrollGridItemVM>();

            foreach (var di in domainList) {
                var item = new Models.Providers.PayrollGridItemVM();
                item.ID = di.ID;
                item.PayrollID = di.PayrollID;
                item.FirstName = di.FirstName;
                item.LastName = di.LastName;
                item.Hours = di.TotalHours;
                item.EntriesMissingCatalystData = di.EntriesMissingCatalystData;
                list.Add(item);
            }

            return list;

        }


        public List<Models.Providers.PayrollGridItemVM> GetPayablesByPeriod(DateTime firstDayOfMonthOfPeriod) {

            return GetPayablesByPeriod(firstDayOfMonthOfPeriod, App.Providers.Models.PayrollFilter.None);
        }


        public List<App.Hours.Models.ScrubOverviewItem> GetScrubOverviewItems(DateTime startDate, DateTime endDate) {

            var data = new Data.Services.CaseService().GetCaseTimeScrubOverviewItems(startDate, endDate);

            var items = new List<App.Hours.Models.ScrubOverviewItem>();

            foreach (var dataItem in data) {
                var item = new App.Hours.Models.ScrubOverviewItem();
                item.ID = dataItem.ID;
                item.FirstName = dataItem.FirstName;
                item.LastName = dataItem.LastName;
                item.ActiveProviders = dataItem.ActiveProviders;
                item.ProvidersFinalized = dataItem.ProvidersFinalized;
                item.ProvidersWithHours = dataItem.ProvidersWithHours;
                item.ScrubbedRecords = dataItem.ScrubbedRecords;
                item.UnscrubbedRecords = dataItem.UnscrubbedRecords;
                item.CommittedRecords = dataItem.CommitedRecords;
                item.BilledRecords = dataItem.BilledRecords;
                item.PaidRecords = dataItem.PaidRecords;
                item.TotalPayable = dataItem.TotalPayable;
                item.TotalBillable = dataItem.TotalBillable;
                item.BCBABillable = dataItem.BCBABillable;
                item.AideBillable = dataItem.AideBillable;
                item.BCBAPercent = dataItem.BCBAPercent;

                items.Add(item);
            }

            return items;

        }

        public List<App.Hours.Models.AvailableDate> GetScrubAvailableDates() {
            DateTime now = DateTime.Now;
            DateTime last = new DateTime(now.Year, now.Month, 1).AddMonths(1);
            DateTime first = last.AddYears(-2);
            DateTime current = last;

            var items = new List<App.Hours.Models.AvailableDate>();
            while (current > first) {
                items.Add(new App.Hours.Models.AvailableDate() { Date = new DateTime(current.Year, current.Month, 1) });
                current = current.AddMonths(-1);
            }

            return items;
        }






        private string connectionString;
        public HoursRepo() {
            connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["CoreConnection"].ConnectionString;
        }

        internal void CommitMHBPeriodExport(DateTime period) {

            using (SqlConnection conn = new SqlConnection(this.connectionString))
                using (SqlCommand cmd = new SqlCommand()) {

                cmd.Connection = conn;
                cmd.Parameters.AddWithValue("@BillingRef", "MBH-XLEXP-" + DateTime.UtcNow.ToString("s"));
                cmd.Parameters.AddWithValue("@Start", period);
                cmd.Parameters.AddWithValue("@End", period.AddMonths(1).AddDays(-1));
                cmd.CommandText = "UPDATE dbo.CaseAuthHours SET HoursBillingRef = @BillingRef WHERE HoursDate >= @Start AND HoursDate <= @End AND HoursBillingRef IS NULL AND HoursStatus = 3;";

                cmd.ExecuteNonQueryToInt();

            }

        }
    }
}