using Dymeng.Framework.Data.SqlClient;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace AABC.Web.Repositories
{
    public class UniversalSearchRepository
    {

        public List<Models.UniversalSearch.EntryItem> GetPatientSearchResult(string searchTerm) {

            using (SqlConnection conn = new SqlConnection(this.connectionString)) 
                using (SqlCommand cmd = new SqlCommand()) {

                cmd.Connection = conn;

                cmd.CommandText = "SELECT c.ID, p.PatientFirstName, p.PatientLastName, p.PatientPhone, p.PatientEmail " +
                    "FROM dbo.Patients AS p " +
                    "INNER JOIN dbo.Cases AS c ON c.PatientID = p.ID " + 
                    "WHERE (c.CaseStatus > -1) AND (PatientFirstName LIKE @SearchTerm OR PatientLastName LIKE @SearchTerm) ";

                cmd.Parameters.AddWithValue("@SearchTerm", searchTerm + "%");

                DataTable table = cmd.GetTable();

                var items = new List<Models.UniversalSearch.EntryItem>();

                foreach (DataRow r in table.Rows) {
                    var item = new Models.UniversalSearch.EntryItem();
                    item.ID = r.ToInt("ID");
                    item.FirstName = r.ToStringValue("PatientFirstName");
                    item.LastName = r.ToStringValue("PatientLastName");
                    item.Phone = r.ToStringValue("PatientPhone");
                    item.Email = r.ToStringValue("PatientEmail");
                    item.Type = "patient";
                    items.Add(item);
                }
                return items;
            }
        }




        public List<Models.UniversalSearch.EntryItem> GetProviderSearchResult(string searchTerm) {

            using (SqlConnection conn = new SqlConnection(this.connectionString))
            using (SqlCommand cmd = new SqlCommand()) {

                cmd.Connection = conn;

                cmd.CommandText = "SELECT ID, ProviderFirstName, ProviderLastName, ProviderPrimaryPhone, ProviderPrimaryEmail " +
                    "FROM dbo.Providers " +
                    "WHERE (ProviderActive = 1) AND (ProviderFirstName LIKE @SearchTerm OR ProviderLastName LIKE @SearchTerm) ";

                cmd.Parameters.AddWithValue("@SearchTerm", searchTerm + "%");

                DataTable table = cmd.GetTable();

                var items = new List<Models.UniversalSearch.EntryItem>();

                foreach (DataRow r in table.Rows) {
                    var item = new Models.UniversalSearch.EntryItem();
                    item.ID = r.ToInt("ID");
                    item.FirstName = r.ToStringValue("ProviderFirstName");
                    item.LastName = r.ToStringValue("ProviderLastName");
                    item.Phone = r.ToStringValue("ProviderPrimaryPhone");
                    item.Email = r.ToStringValue("ProviderPrimaryEmail");
                    item.Type = "provider";
                    items.Add(item);
                }
                return items;
            }
        }


        public List<Models.UniversalSearch.EntryItem> GetReferralSearchResult(string searchTerm) {

            using (SqlConnection conn = new SqlConnection(this.connectionString))
            using (SqlCommand cmd = new SqlCommand()) {

                cmd.Connection = conn;

                cmd.CommandText = "SELECT ID, ReferralFirstName, ReferralLastName, ReferralPhone, ReferralEmail " +
                    "FROM dbo.Referrals " +
                    "WHERE (ReferralActive = 1) AND (ReferralFirstName LIKE @SearchTerm OR ReferralLastName LIKE @SearchTerm) ";

                cmd.Parameters.AddWithValue("@SearchTerm", searchTerm + "%");

                DataTable table = cmd.GetTable();

                var items = new List<Models.UniversalSearch.EntryItem>();

                foreach (DataRow r in table.Rows) {
                    var item = new Models.UniversalSearch.EntryItem();
                    item.ID = r.ToInt("ID");
                    item.FirstName = r.ToStringValue("ReferralFirstName");
                    item.LastName = r.ToStringValue("ReferralLastName");
                    item.Phone = r.ToStringValue("ReferralPhone");
                    item.Email = r.ToStringValue("ReferralEmail");
                    item.Type = "referral";
                    items.Add(item);
                }
                return items;
            }
        }



        private string connectionString = "";
        public UniversalSearchRepository() {
            this.connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["CoreConnection"].ConnectionString;
        }
    }
}