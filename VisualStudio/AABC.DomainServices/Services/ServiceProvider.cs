using AABC.Domain2.Providers;
using AABC.Domain2.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AABC.DomainServices.Services
{

    /* Use this to obtain all services
     * 
     * The original service system wasn't driven by insurance, only by provider type.
     * The new system will be driven by insurance and provider type.
     * Any existing service list fetches should be re-routed to this class.
     * 
     */

    public class ServiceProvider
    {

        

        public List<Service> GetAllServices() {
            return _context.Services.ToList();
        }

        public List<Service> GetServicesByType(ServiceTypes type) {
            return _context.Services.Where(x => x.Type == type).ToList();
        }



        public List<Service> GetServices(Domain2.Insurances.Insurance insurance, int providerTypeID, DateTime refDate) {

            if (insurance == null) {
                return new List<Service>();
            }

            var serviceIDs = insurance.Services
                .Where(x => (x.EffectiveDate == null || x.EffectiveDate <= refDate)
                            && (x.DefectiveDate == null || x.DefectiveDate >= refDate)
                            && x.ProviderTypeID == providerTypeID)
                .Select(x => x.ServiceID)
                .ToArray();

            return _context.Services.Where(x => serviceIDs.Contains(x.ID)).ToList();
        }

        public List<Service> GetServices(Domain2.Insurances.Insurance insurance, int providerTypeID) {
            return GetServices(insurance, providerTypeID, DateTime.Now);
        }
        
        public List<Service> GetServices(Domain2.Cases.Case @case, Provider provider) {
            return GetServices(@case, provider, DateTime.Now);
        }

        public List<Service> GetServices(Domain2.Cases.Case @case, Provider provider, DateTime refDate) {
            var insurance = @case.GetActiveInsuranceAtDate(refDate)?.Insurance;
            var providerTypeID = provider.ProviderTypeID;
            return GetServices(insurance, providerTypeID, refDate);
        }




        #region LegacyServices
        public List<Service> GetLegacyServicesByProviderType(int providerTypeID) {

            var q = from pts in _context.ProviderTypeServices 
                    where pts.ProviderTypeID == providerTypeID
                    select pts.Service;

            return q.ToList();
        }

        public List<Service> GetLegacyServicesByProvider(int providerID) {
            var provider = _context.Providers.Find(providerID);
            return GetLegacyServicesByProviderType(provider.ProviderTypeID);
        }

        public List<Service> GetLegacyServicesByProvider(Provider provider) {
            return GetLegacyServicesByProviderType(provider.ProviderTypeID);
        }

        public List<Service> GetLegacyServicesByProviderType(ProviderType providerType) {
            return GetLegacyServicesByProviderType(providerType.ID);
        }
        
        public List<Service> GetLegacyServicesByProviderType(ProviderTypeIDs providerTypeID) {
            return GetLegacyServicesByProviderType((int)providerTypeID);
        }
        #endregion







        private Data.V2.CoreContext _context;

        public ServiceProvider() {
            _context = ContextProvider.Context;
        }
        public ServiceProvider(Data.V2.CoreContext context) {
            _context = context;
        }

    }
}
