using Dymeng.Framework.Data.SqlClient;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace AABC.Web.Repos
{
    public class DataListsRepo
    {


        public bool DeleteAuthDetail(int id) {

            if (!DomainServices.Authorizations.Validations.AuthorizationIsDeletable(id)) {
                return false;
            }

            using (SqlConnection conn = new SqlConnection(this.connectionString))
            using (SqlCommand cmd = new SqlCommand()) {

                cmd.Connection = conn;

                cmd.CommandText = "DELETE FROM dbo.AuthCodes WHERE ID = @ID;";

                cmd.Parameters.AddWithValue("@ID", id);

                cmd.ExecuteNonQueryToInt();

                return true;
            }

            

        }


        public void CreateAuthDetail(string code, string desc) {

            using (SqlConnection conn = new SqlConnection(this.connectionString))
            using (SqlCommand cmd = new SqlCommand()) {

                cmd.Connection = conn;

                cmd.CommandText = "INSERT INTO dbo.AuthCodes (CodeCode, CodeDescription) VALUES (@Code, @Desc);";

                cmd.Parameters.AddWithValue("@Code", code);
                cmd.Parameters.AddWithValue("@Desc", desc);

                cmd.InsertToIdentity();
            }
        }


        public void EditAuthDetail(int id, string code, string desc) {

            using (SqlConnection conn = new SqlConnection(this.connectionString))
                using (SqlCommand cmd = new SqlCommand()) {

                cmd.Connection = conn;

                cmd.CommandText = "UPDATE dbo.AuthCodes SET CodeCode = @Code, CodeDescription = @Desc WHERE ID = @ID;";

                cmd.Parameters.AddWithValue("@Code", code);
                cmd.Parameters.AddWithValue("@Desc", desc);
                cmd.Parameters.AddWithValue("@ID", id);

                cmd.ExecuteNonQueryToInt();
            }
        }


        public Models.DataLists.AuthDetailVM GetAuthDetail(int authID) {

            var domainAuth = service.GetAuthorization(authID);
            var auth = new Models.DataLists.AuthDetailVM();

            auth.AuthDetailEditID = domainAuth.ID;
            auth.AuthDetailEditCode = domainAuth.Code;
            auth.AuthDetailEditDescription = domainAuth.Description;

            return auth;

        }

        


        public List<Models.DataLists.AuthGridListItem> GetAuthGridListItems() {
            var domainAuths = service.GetAuthorizations();

            var list = new List<Models.DataLists.AuthGridListItem>();

            foreach (var auth in domainAuths) {
                var item = new Models.DataLists.AuthGridListItem();
                item.ID = auth.ID.Value;
                item.Code = auth.Code;
                item.Description = auth.Description;
                list.Add(item);
            }

            return list;

        }



        private Data.Services.DataListService service;
        private string connectionString;

        public DataListsRepo() {
            service = new Data.Services.DataListService();
            connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["CoreConnection"].ConnectionString;
        }


    }
}