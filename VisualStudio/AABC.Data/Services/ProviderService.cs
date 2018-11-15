using AABC.Data.Models;
using System.Collections.Generic;
using System.Linq;

namespace AABC.Data.Services
{
    public class ProviderService
    {


        public List<Domain.Providers.Provider> GetProvidersByCase(int caseID) {
            
            var context = new CoreEntityModel();
            
            var q = from providers in context.Providers
                    join caseproviders in context.CaseProviders on providers.ID equals caseproviders.ProviderID
                    where caseproviders.CaseID == caseID
                    select providers;

            var entities = q.ToList();

            return Mappings.ProviderMappings.Providers(entities).ToList();
            
        }
        

        public Domain.Providers.Provider GetProvider(int id) {

            var context = new Models.CoreEntityModel();
            var data = context.Providers.Find(id);

            if (data == null) {
                return null;
            } else {
                var provider = Mappings.ProviderMappings.Provider(data);
                var entity = context.ProviderPortalUsers.Where(x => x.ProviderID == provider.ID.Value).FirstOrDefault();
                if (entity != null) {
                    provider.PortalUser = Mappings.ProviderPortalMappings.ProviderPortalUser(entity, false);
                }
                return provider;
            }
            
        }

        
        public bool ProviderNumberExists(string providerNumber) {

            var context = new Models.CoreEntityModel();

            var data = context.Providers.Where(x => x.ProviderNPI == providerNumber || x.ProviderNumber == providerNumber).FirstOrDefault();

            return data == null ? false : true;
            
        }

        public IEnumerable<Domain.Providers.Provider> GetActiveProviders() {

            var context = new Models.CoreEntityModel();

            var data = context.Providers.ToList();

            return Mappings.ProviderMappings.Providers(data);            

        }

        public IEnumerable<Domain.General.GeneralLanguage> GetCommonLanguages() {

            var cache = CacheService.Get(CacheServiceItems.CommonLanguageList);

            if (cache == null) {
                var context = new Models.CoreEntityModel();
                var data = context.CommonLanguages.ToList();
                var models = Mappings.GeneralMappings.GeneralLanguages(data);
                CacheService.Add(CacheServiceItems.CommonLanguageList, models, 120);
                return models;
            } else {
                return cache as List<Domain.General.GeneralLanguage>;
            }
            
        }




        


    }
}
