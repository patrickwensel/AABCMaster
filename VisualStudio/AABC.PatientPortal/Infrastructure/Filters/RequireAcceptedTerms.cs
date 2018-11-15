using AABC.PatientPortal.App.Terms;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;

namespace AABC.PatientPortal.Infrastructure.Filters
{
    public class RequireAcceptedTerms : IActionFilter
    {
        private List<RouteDescriptor> AllowedRoutes;
        private readonly TermsService TermsService;

        public RequireAcceptedTerms()
        {
            AllowedRoutes = new List<RouteDescriptor>();
            AllowedRoutes.Add(new RouteDescriptor { Controller = "Terms", Action = "Index" });
            AllowedRoutes.Add(new RouteDescriptor { Controller = "Terms", Action = "Accept" });
            AllowedRoutes.Add(new RouteDescriptor { Controller = "Account", Action = "Logout" });
            AllowedRoutes.Add(new RouteDescriptor { Controller = "Payments", Action = "PaySchedules " });
            TermsService = new TermsService();
        }

        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!filterContext.HttpContext.Request.IsAuthenticated)
            {
                return;
            }
            var controller = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName.ToLower();
            var action = filterContext.ActionDescriptor.ActionName.ToLower();
            if (AllowedRoutes.Any(m => m.Controller.ToLower() == controller && m.Action.ToLower() == action))
            {
                return;
            }
            if (!TermsService.UserHasAcceptedLatestTerms())
            {
                filterContext.Result = new RedirectToRouteResult(
                    new RouteValueDictionary(new { controller = "Terms", action = "Index" })
                );
            }
        }

        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
        }

        private class RouteDescriptor
        {
            public string Controller { get; set; }
            public string Action { get; set; }
        }
    }
}