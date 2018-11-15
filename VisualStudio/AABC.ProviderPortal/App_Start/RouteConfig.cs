using System.Web.Mvc;
using System.Web.Routing;

namespace AABC.ProviderPortal
{

    public class RouteConfig {

        public static void RegisterRoutes(RouteCollection routes) {

            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.IgnoreRoute("{resource}.ashx/{*pathInfo}");

            routes.MapMvcAttributeRoutes();

            //routes.MapRoute(
            //    name: "ProviderTypes",
            //    url: "Providers/Types/{action}/{id}",
            //    defaults: new { controller = "ProviderTypes", action = "Index", id = UrlParameter.Optional }
            //);

            routes.MapMvcAttributeRoutes();
            
            routes.MapRoute(
                name: "CaseManager", // Route name
                url: "Case/{id}/Manage/{view}", // URL with parameters
                defaults: new { controller = "Case", action = "ManageDirectRoute" } // Parameter defaults
            );

            routes.MapRoute(
                name: "Default", // Route name
                url: "{controller}/{action}/{id}", // URL with parameters
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional } // Parameter defaults
            );


        }
    }
}