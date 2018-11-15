using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Http;

namespace AABC.Mobile.Api
{
    public class CurrentUserProvider : ICurrentUserProvider {

        Data.V2.CoreContext _dbContext;
            

        public CurrentUserProvider(IDBContextProvider _dbContextProvider) {
            _dbContext = _dbContextProvider.CoreContext;
        }

        public int AspNetUserID {
            get {
                var identity = (ClaimsIdentity)HttpContext.Current.User.Identity;
                IEnumerable<Claim> claims = identity.Claims;

                // get the userid claim 
                var idClaim = claims.First(c => c.Type == ClaimTypes.Sid);
                var userId = Convert.ToInt32(idClaim.Value);

                return userId;
            }
        }

        public int ProviderID {
            get {
                return Provider.ID;
            }
        }

        public Domain2.Providers.Provider Provider {
            get {

                var userID = AspNetUserID;
                
                // ProviderPortalUsers not set up in domain, let's tap direct to the db instead
                var connectionString = _dbContext.Database.Connection.ConnectionString;

                using (var conn = new System.Data.SqlClient.SqlConnection(connectionString))
                using (var cmd = new System.Data.SqlClient.SqlCommand()) {

                    cmd.Connection = conn;
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.CommandText = "SELECT TOP 1 ProviderID FROM dbo.ProviderPortalUsers WHERE AspNetUserID = @UserID";
                    cmd.Parameters.AddWithValue("@UserID", userID);

                    conn.Open();
                    var result = cmd.ExecuteScalar();
                    conn.Close();

                    return _dbContext.Providers.Find(int.Parse(result.ToString()));
                }

            }
        }
        
    }
}