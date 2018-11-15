using System;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace AABC.Web
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            AuthConfig.RegisterAuth();
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            ModelBinders.Binders.DefaultBinder = new DevExpress.Web.Mvc.DevExpressEditorsBinder();

            DevExpress.Web.ASPxWebControl.CallbackError += Application_Error;

            ViewEngines.Engines.Clear();
            ViewEngines.Engines.Add(new Infrastructure.ViewEngine.DymengRazorViewEngine());

            DomainServices.ContextProvider.SetContextProviderFunction(() => { return AppService.Current.DataContextV2; });

            // AntiForgeryConfig.SuppressIdentityHeuristicChecks = true


        }

        protected void Session_Start(Object sender, EventArgs e)
        {
            if (Request.IsAuthenticated)
            {
                Console.WriteLine("Here");
            }

        }

        protected void Application_PostAuthenticateRequest(Object sender, EventArgs e)
        {
            Console.WriteLine("Here");
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            var exception = HttpContext.Current.Server.GetLastError();
            Task.Factory.StartNew(() =>
            {
                Dymeng.Framework.Exceptions.Handle(exception);
            });
            //TODO: Handle Exception
        }
    }
}