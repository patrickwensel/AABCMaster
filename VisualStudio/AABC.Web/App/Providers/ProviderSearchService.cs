using AABC.Cache;
using AABC.Data.V2;
using AABC.Web.Models.Providers;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;

namespace AABC.Web.App.Providers
{
    public class ProviderSearchService
    {
        private readonly CoreContext Context;
        private readonly IHashCacheRepository<ProvidersListItemVM, int> CacheRepository;

        public ProviderSearchService(CoreContext context)
        {
            int minutes = int.TryParse(ConfigurationManager.AppSettings["Cache.Providers.Expiration"] as string, out minutes) ? minutes : 60 * 24;
            Context = context;
            CacheRepository = new HashCacheRepository<ProvidersListItemVM, int>(AppService.Current.CacheClient, m => m.ID, minutes);
        }

        public IEnumerable<ProvidersListItemVM> GetAll()
        {
            IEnumerable<ProvidersListItemVM> results;
            if (!CacheRepository.IsLoaded())
            {
                results = GetProviders(null);
                CacheRepository.SetAll(results);
                return results;
            }
            return CacheRepository.GetAll();
        }

        public void UpdateEntry(int id)
        {
            UpdateEntries(new int[] { id });
        }

        public void UpdateEntries(IEnumerable<int> ids)
        {
            if (CacheRepository.IsLoaded())
            {
                var elements = new List<ProvidersListItemVM>();
                foreach (var id in ids)
                {
                    var a = GetProviders(id).SingleOrDefault();
                    if (a != null)
                    {
                        elements.Add(a);
                    }
                }
                CacheRepository.SetAll(elements);
            }
        }


        public void Remove(int id)
        {
            CacheRepository.Remove(id);
        }


        private IEnumerable<ProvidersListItemVM> GetProviders(int? id)
        {
            var sql = "EXEC dbo.GetProvidersSearch";
            if (id.HasValue)
            {
                return Context.Database.SqlQuery<ProvidersListItemVM>($"{sql} @providerId", new SqlParameter("providerId", id.Value));
            }
            return Context.Database.SqlQuery<ProvidersListItemVM>(sql);
        }

    }
}