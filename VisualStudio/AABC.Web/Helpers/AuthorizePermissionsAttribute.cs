using AABC.Domain.Admin;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace AABC.Web.Helpers
{
    /// <summary>
    /// Allows/restricts user access based on permissions.
    /// </summary>
    public class AuthorizePermissionsAttribute : AuthorizeAttribute
    {
        private readonly Permissions[] _permissions;

        public AuthorizePermissionsAttribute(params Permissions[] permissions)
        {
            _permissions = permissions;
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            var isAuthorized = base.AuthorizeCore(httpContext);
            if (!isAuthorized)
            {
                return false;
            }

            foreach (var permission in _permissions)
            {
                if (Global.Default.User().HasPermission(permission))
                {
                    return true;
                }
            }

            return false;
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            if (filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                filterContext.Result = new RedirectToRouteResult(
                    new RouteValueDictionary(
                        new
                        {
                            controller = "Home",
                            action = "Index"
                        })
                );
            }
            else
            {
                base.HandleUnauthorizedRequest(filterContext);
            }
        }
    }
}