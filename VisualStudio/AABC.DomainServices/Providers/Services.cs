using AABC.Domain2.Providers;
using AABC.Domain2.Services;
using System;
using System.Collections.Generic;
using System.Linq;


namespace AABC.DomainServices.Providers
{
    public class Services
    {


        private static Data.V2.CoreContext _context { get { return ContextProvider.Context; } }


        [Obsolete("Obsolete - DomainServices.Services.ServiceProvider.GetLegacyServicesByProviderType() instead")]
        public static List<Service> GetServices(ProviderTypeIDs typeID) {
            var sp = new DomainServices.Services.ServiceProvider(_context);
            return sp.GetLegacyServicesByProviderType(typeID);
        }
        




        [Obsolete("Obsolete - use the domain2 returning GetServices(ProviderTypeIDs typeID) instead")]
        public static List<Domain.Cases.Service> GetServices(int? providerTypeID, bool returnSSG = false) {

            var caseService = new Data.Services.CaseService();

            List<Domain.Cases.Service> services;

            if (providerTypeID.HasValue) {
                services = caseService.GetServicesByProviderType(providerTypeID.Value);
            } else {
                services = caseService.GetServices();
            }

            if (returnSSG) {
                return services;
            } else {
                return services.Where(x => x.Code != "SSG").ToList();
            }

        }

    }
}
