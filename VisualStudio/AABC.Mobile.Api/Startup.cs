using System;
using System.Configuration;
using System.Linq;
using System.Net.Http.Formatting;
using System.Web.Http;

using EasyJwtAuth;
using Newtonsoft.Json.Serialization;
using Owin;

using AABC.Mobile.Api.App_Start;
using AABC.Mobile.Api.Providers;
using WebMatrix.WebData;

namespace AABC.Mobile.Api
{
	public class Startup
	{
		private readonly string audienceId = ConfigurationManager.AppSettings["audienceId"];
		private readonly string issuer = ConfigurationManager.AppSettings["issuer"];
		private readonly string secret = ConfigurationManager.AppSettings["secret"];

		public void Configuration(IAppBuilder app)
		{
			// make sure we're logging
			log4net.Config.XmlConfigurator.Configure();

			// configure http
			HttpConfiguration httpConfig = new HttpConfiguration();

			// add the filters (e.g. requiring https)
			FilterConfig.RegisterHttpFilters(httpConfig.Filters);

			// add the handlers (e.g. logging and exception handling)
			LoggingConfig.RegisterHandlers(httpConfig.Services);

			WebSecurity.InitializeDatabaseConnection("ProviderPortalConnection", "UserProfile", "UserId", "UserName", autoCreateTables: true);

            UnityConfig.RegisterComponents(httpConfig);

			ConfigureOAuth(app, (ISettingsProvider)httpConfig.DependencyResolver.GetService(typeof(ISettingsProvider)));

			ConfigureWebApi(httpConfig);

			app.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);

			app.UseWebApi(httpConfig);

		}


		void ConfigureOAuth(IAppBuilder app, ISettingsProvider settings)
		{
			// set up easyJwtAuthorizationServerOptions

			var easyJwtAuthorizationServerOptions = new EasyJwtAuthorizationServerOptions();
#if DEBUG
			// http only allowed in debug
			easyJwtAuthorizationServerOptions.AllowInsecureHttp = true;
#else
			easyJwtAuthorizationServerOptions.AllowInsecureHttp = false;
#endif
			easyJwtAuthorizationServerOptions.TokenEndpointPath = "/oauth2/token";
			easyJwtAuthorizationServerOptions.AccessTokenExpireTimeSpan = TimeSpan.FromDays(1);

			var easyJwtTokenOptions = new EasyJwtTokenOptions(audienceId, issuer, secret);
			app.UseEasyJwtAuthorizationServer(easyJwtAuthorizationServerOptions, new CustomOAuthProvider(settings.ServerMode_Demo_Enabled), easyJwtTokenOptions);
			app.UseEasyJwtAuthentication(easyJwtTokenOptions);
		}

		void ConfigureWebApi(HttpConfiguration config)
		{
			config.MapHttpAttributeRoutes();

			var jsonFormatter = config.Formatters.OfType<JsonMediaTypeFormatter>().First();
			jsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
		}
	}
}