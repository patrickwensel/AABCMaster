using AABC.Data.V2;
using ATrack.Authenticator;
using ATrack.ServiceRegistration.Unity;
using System.Configuration;
using Unity;
using Unity.Injection;
using Unity.Lifetime;

namespace AABC.ATrack.Authenticator.ProviderApp
{
    public class ServiceRegistrator : BaseServiceRegistrator
    {
        public ServiceRegistrator(IUnityContainer container) : base(container)
        {
        }

        protected override void RegisterComponents(IUnityContainer container)
        {
            container.TryRegisterType<CoreContext>(new PerThreadLifetimeManager());
            container.TryRegisterType<IUserProvider, UserProvider>(new PerThreadLifetimeManager(), 
                new InjectionConstructor(
                    GetProviderPortalConnectionString(),
                    new ResolvedParameter<CoreContext>()
                )
            );
            container.TryRegisterSingleton<IUserProviderFactory, UserProviderFactory>();
        }

        private static string GetProviderPortalConnectionString()
        {
            return ConfigurationManager.ConnectionStrings["ProviderPortalConnection"].ConnectionString;
        }
    }
}
