using AABC.Data.V2;
using AABC.DomainServices.Providers;
using ATrack.Integrators.ProviderPortal.Contracts;
using ATrack.ServiceRegistration.Unity;
using System.Configuration;
using Unity;
using Unity.Injection;
using Unity.Lifetime;

namespace AABC.ATrack.Integrators.ProviderPortal
{
    public class ServiceRegistrator : BaseServiceRegistrator
    {
        public ServiceRegistrator(IUnityContainer container) : base(container)
        {
        }

        protected override void RegisterComponents(IUnityContainer container)
        {
            container.TryRegisterType<CoreContext>(new PerThreadLifetimeManager());
            container.TryRegisterType<ProviderProvider>(new PerThreadLifetimeManager());
            container.TryRegisterType<ICaseIntegrator, CaseIntegrator>(new PerThreadLifetimeManager(), 
                new InjectionConstructor(
                    new ResolvedParameter<CoreContext>(),
                    new ResolvedParameter<ProviderProvider>(),
                    GetVisibleAfterEndDateDays()
                )
            );
        }

        private static int GetVisibleAfterEndDateDays()
        {
            var s = ConfigurationManager.AppSettings["Providers.Cases.VisibleAfterEndDateDays"];
            int.TryParse(s, out int days);
            return days;
        }
    }
}

