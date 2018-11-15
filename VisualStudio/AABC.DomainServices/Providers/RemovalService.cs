using Dymeng.Framework.Data.SqlClient;
using System.Data.SqlClient;

namespace AABC.DomainServices.Providers
{
    public class RemovalService
    {

        Data.V2.CoreContext _context;

        public RemovalService(Data.V2.CoreContext context) {
            _context = context;
        }

        public bool DeleteProvider(int providerID, string providerPortalUsersConnectionString) {

            var provider = _context.Providers.Find(providerID);

            if (provider == null) {
                return false;
            }

            if (provider.Hours.Count > 0) {
                return false;
            }

            if (provider.Finalizations.Count > 0) {
                return false;
            }

            if (provider.Cases.Count > 0) {
                return false;
            }

            RemoveProviderUser(providerID, providerPortalUsersConnectionString);

            _context.Providers.Remove(provider);
            _context.SaveChanges();

            return true;

        }

        public void RemoveProviderUser(int providerID, string providerPortalUsersConnectionString) {

            var legacyProviderService = new Data.Services.ProviderService();
            var legacyProvider = legacyProviderService.GetProvider(providerID);

            if (legacyProvider.PortalUser == null) {
                return;
            }

            var legacyPortalUser = legacyProvider.PortalUser;

            using (SqlConnection conn = new SqlConnection(providerPortalUsersConnectionString))
            using (SqlCommand cmd = new SqlCommand()) {
                cmd.Connection = conn;
                cmd.CommandText =
                    "DELETE FROM dbo.UserProfile WHERE UserId = @AspNetID;" +
                    "DELETE FROM dbo.webpages_Membership WHERE UserId = @AspNetID;";
                cmd.Parameters.AddWithValue("@AspNetID", legacyPortalUser.AspNetUserID);
                cmd.ExecuteNonQueryToInt();
            }

            var portalUser = _context.ProviderPortalUsers.Find(legacyPortalUser.ID);
            if (portalUser != null) {
                _context.ProviderPortalUsers.Remove(portalUser);
                _context.SaveChanges();
            }
            
        }


    }
}
