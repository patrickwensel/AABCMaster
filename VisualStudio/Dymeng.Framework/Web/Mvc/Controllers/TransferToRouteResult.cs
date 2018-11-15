using System;
using System.Web.Mvc;
using System.Web.Routing;

namespace Dymeng.Framework.Web.Mvc.Controllers
{

    // http://stackoverflow.com/questions/799511/how-to-simulate-server-transfer-in-asp-net-mvc?lq=1

    public class TransferToRouteResult : ActionResult
    {
        public string RouteName { get; set; }
        public RouteValueDictionary RouteValues { get; set; }

        public TransferToRouteResult(RouteValueDictionary routeValues)
            : this(null, routeValues) {
        }

        public TransferToRouteResult(string routeName, RouteValueDictionary routeValues) {
            this.RouteName = routeName ?? string.Empty;
            this.RouteValues = routeValues ?? new RouteValueDictionary();
        }

        public override void ExecuteResult(ControllerContext context) {
            if (context == null)
                throw new ArgumentNullException("context");

            var urlHelper = new UrlHelper(context.RequestContext);
            var url = urlHelper.RouteUrl(this.RouteName, this.RouteValues);

            var actualResult = new TransferResult(url);
            actualResult.ExecuteResult(context);
        }
    }
}
