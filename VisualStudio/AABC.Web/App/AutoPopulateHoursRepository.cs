using AABC.Domain.Cases;
using Dymeng.Framework.Data.SqlClient;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace AABC.Web.Repositories
{
    public class AutoPopulateHoursRepository
    {


        private readonly string connectionString;
        Data.Services.CaseService caseService;

        public AutoPopulateHoursRepository() {
            caseService = new Data.Services.CaseService();
            connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["CoreConnection"].ConnectionString;
        }


        public void AddAutopopulatedHours(List<Domain.Cases.CaseAuthorizationHours> items) {

            if (items.Count == 0) {
                return;
            }

            foreach (var item in items) {
                SaveCaseAuthHours(item, item.CaseID.Value, item.Authorization?.ID.Value, item.ProviderID);
            }

        }



        public Case GetCase(int caseID) {
            return caseService.GetCase(caseID);
        }

        public List<CaseAuthorization> GetCaseAuthorizations(int caseID) {
            return caseService.GetCaseAuthorizationsAndHours(caseID);
        }





        public void SaveCaseAuthHours(CaseAuthorizationHours hours, int caseID, int? authID, int providerID) {

            using (SqlConnection conn = new SqlConnection(this.connectionString))
            using (SqlCommand cmd = new SqlCommand()) {

                cmd.Connection = conn;

                cmd.CommandText = "INSERT INTO dbo.CaseAuthHours (" +
                    "CaseAuthID, CaseID, CaseProviderID, HoursDate, HoursTimeIn, HoursTimeOut, HoursTotal, HoursServiceID, HoursNotes, HoursStatus, HoursPayable, HoursBillable " +
                    ") VALUES (" +
                    "@AuthID, @CaseID, @ProviderID, @Date, @TimeIn, @TimeOut, @Hours, @ServiceID, @Notes, @Status, @Payable, @Billable);";

                cmd.Parameters.AddWithValue("@ProviderID", providerID);
                cmd.Parameters.AddWithValue("@Date", hours.Date);
                cmd.Parameters.AddWithValue("@TimeIn", hours.TimeIn);
                cmd.Parameters.AddWithValue("@TimeOut", hours.TimeOut);
                cmd.Parameters.AddWithValue("@Hours", hours.HoursTotal);
                cmd.Parameters.AddWithValue("@ServiceID", hours.Service.ID.Value);
                cmd.Parameters.AddWithValue("@Notes", hours.Notes);
                if (authID.HasValue) {
                    cmd.Parameters.AddWithValue("@AuthID", authID.Value);
                } else {
                    cmd.Parameters.AddWithNullableValue("@AuthID", null);
                }
                cmd.Parameters.AddWithValue("@CaseID", caseID);
                cmd.Parameters.AddWithValue("@Status", hours.Status);
                cmd.Parameters.AddWithNullableValue("@Payable", hours.PayableHours);
                cmd.Parameters.AddWithNullableValue("@Billable", hours.BillableHours);

                var id = cmd.InsertToIdentity();

                ProcessAuthBreakdown(id);

            }

        }

        private void ProcessAuthBreakdown(int hoursID) {

            // now that the hours are updated, re-calc the auth breakdown
            var c2 = AppService.Current.DataContextV2;
            var h = c2.Hours.Find(hoursID);
            var authResolver = new DomainServices.Hours.AuthResolver(h);
            authResolver.UpdateAuths(c2);

        }


    }
}