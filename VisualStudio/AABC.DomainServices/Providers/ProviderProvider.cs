using AABC.Data.V2;
using AABC.Domain2.Providers;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace AABC.DomainServices.Providers
{
    public class ProviderProvider
    {
        private readonly CoreContext Context;

        public ProviderProvider(CoreContext context)
        {
            Context = context;
        }

        public Provider GetProvider(int userProviderID)
        {
            int providerID;
            // ProviderPortalUsers not set up in domain, let's tap direct to the db instead
            using (var conn = new SqlConnection(Context.Database.Connection.ConnectionString))
            using (var cmd = new SqlCommand())
            {
                cmd.Connection = conn;
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "SELECT TOP 1 ProviderID FROM dbo.ProviderPortalUsers WHERE AspNetUserID = @UserID";
                cmd.Parameters.AddWithValue("@UserID", userProviderID);
                conn.Open();
                providerID = int.Parse(cmd.ExecuteScalar().ToString());
                conn.Close();
            }
            return Context.Providers.SingleOrDefault(m => m.ID == providerID);
        }
    }
}
