using System;
using System.Web.Mvc;
using System.Web.Routing;

namespace AABC.ProviderPortal
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            //GlobalConfiguration.Configure(WebApiConfig.Register);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            AuthConfig.RegisterAuth();
            
            ModelBinders.Binders.DefaultBinder = new DevExpress.Web.Mvc.DevExpressEditorsBinder();

            DevExpress.Web.ASPxWebControl.CallbackError += Application_Error;

            System.Web.Helpers.AntiForgeryConfig.SuppressIdentityHeuristicChecks = true;
            
            DomainServices.ContextProvider.SetContextProviderFunction(() => { return AppService.Current.Context; });
        }

        protected void Application_Error(object sender, EventArgs e) 
        {
            Exception exception = System.Web.HttpContext.Current.Server.GetLastError();
            Dymeng.Framework.Exceptions.Handle(exception, Global.GetWebInfo());
            //TODO: Handle Exception
        }
    }
}