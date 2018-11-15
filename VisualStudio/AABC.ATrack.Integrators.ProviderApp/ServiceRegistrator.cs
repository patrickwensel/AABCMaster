using AABC.Data.V2;
using AABC.DomainServices.Providers;
using ATrack.Integrators.ProviderApp.Contracts;
using ATrack.ServiceRegistration.Unity;
using Unity;
using Unity.Lifetime;

namespace AABC.ATrack.Integrators.ProviderApp
{
    public class ServiceRegistrator : BaseServiceRegistrator
    {
        public ServiceRegistrator(IUnityContainer unityContainer) : base(unityContainer) { }

        protected override void RegisterComponents(IUnityContainer container)
        {
            container.TryRegisterType<CoreContext>(new PerThreadLifetimeManager()); //per request
            container.TryRegisterType<ProviderProvider>(new PerThreadLifetimeManager());
            container.TryRegisterType<IIntegrator, Integrator>(new HierarchicalLifetimeManager());//per request
        }
    }
}
