using AABC.Domain.ProviderPortal;
using AABC.Domain.Providers;
using System.Collections.Generic;
using System.Linq;

namespace AABC.Data.Services
{
    public class ProviderPortalService
    {


        public Provider GetProvider(int providerID) {
            var providerService = new ProviderService();
            return providerService.GetProvider(providerID);
        }

        public List<Provider> GetProviders() {

            var context = new Models.CoreEntityModel();

            var entityProviders = context.Providers.ToList();
            var entityProviderUsers = context.ProviderPortalUsers.ToList();

            var providers = Mappings.ProviderMappings.Providers(entityProviders);
            var users = Mappings.ProviderPortalMappings.ProviderPortalUsers(entityProviderUsers);

            foreach (var p in providers) {
                p.PortalUser = users.Where(x => x.ProviderID == p.ID).FirstOrDefault();
            }
            
            return providers.ToList();
        }

        public ProviderPortalUser GetPortalUserByAspNetUsername(string aspNetUsername) {

            var context = new Models.CoreEntityModel();

            var entity = context.ProviderPortalUsers.Where(x => x.ProviderUserNumber == aspNetUsername).FirstOrDefault();
            if (entity == null) {
                return null;
            }

            var ppu = Mappings.ProviderPortalMappings.ProviderPortalUser(entity, false);
            var provider = context.Providers.Find(ppu.ProviderID);
            ppu.FirstName = provider.ProviderFirstName;
            ppu.LastName = provider.ProviderLastName;

            return ppu;
        }


        

    }
}
