using System.Web.Http;
using Unity;
using Unity.WebApi;
using AABC.Mobile.Api.Repositories;
using AABC.Mobile.Api.Providers;

namespace AABC.Mobile.Api
{
    public static class UnityConfig
    {
        public static void RegisterComponents(HttpConfiguration httpConfig)
        {
			var container = new UnityContainer();

            // register all your components with the container here
            // it is NOT necessary to register your controllers

            // e.g. container.RegisterType<ITestService, TestService>();



            ISettingsProvider settingsRepository = new SettingsProvider();
            container.RegisterInstance<ISettingsProvider>(settingsRepository);

            container.RegisterType<IDBContextProvider, DBContextProvider>();
            container.RegisterType<ICurrentUserProvider, CurrentUserProvider>();
            
            if (settingsRepository.ServerMode_Demo_Enabled) {
                container.RegisterType<ICasesRepository, CasesDemoRepository>();
            } else {
                container.RegisterType<ICasesRepository, CasesRepository>();
            }
            
            httpConfig.DependencyResolver = new UnityDependencyResolver(container);
        }
    }
}