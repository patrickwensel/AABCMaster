using AABC.DomainServices.Patients;
using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
namespace AABC.PatientPortal
{
    public class MvcApplication : HttpApplication
    {
        protected void Application_Start() {

            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            // use a custom view engine to get away from the flat file crap default
            ViewEngineConfig.RegisterViewEngine(new Dymeng.Web.Mvc.DymengRazorViewEngine());
            
        }

        protected void Session_Start(object sender, EventArgs e) {
            Session["init"] = 0;
            if (HttpContext.Current.User.Identity.IsAuthenticated)
            {
                SignInService svcSignIn = new SignInService(AppService.Current.Data.Context);
                int UserId = svcSignIn.GetUserId(HttpContext.Current.User.Identity.Name);
                svcSignIn.Save(new SignInEx() { UserId = UserId, SignInDate = DateTime.Now, SignInType = "Remember" });
            }
        }
    }
}
