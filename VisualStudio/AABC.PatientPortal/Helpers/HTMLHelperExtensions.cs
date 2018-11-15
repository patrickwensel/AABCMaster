using System;
using System.Web.Mvc;

namespace AABC.PatientPortal.Helpers
{


    public static class HMTLHelperExtensions
    {


        public static AppService App(this HtmlHelper html) {
            return AppService.Current;
        }



        //public static bool HasRole(this HtmlHelper html, Domain.Users.Roles role) {

        //    return AppService.Current.User.CurrentUser.IsInRole(role);

        //}


        /// <summary>
        /// Checks the Path portion of the current Request context and matches it to one or more navigation keys
        /// (one or more meaning both top and second level navigation matches)
        /// </summary>
        /// <param name="html">extension object</param>
        /// <param name="navKey">Navigation Key that we need to check for a matching path on</param>
        /// <param name="cssClass">CSS Class to return if the path matches the specified navigation key</param>
        /// <returns></returns>
        public static string IsSelectedByUrl(this HtmlHelper html, App.Shared.Navigation.NavigationKey navKey, string cssClass = null) {

            if (string.IsNullOrEmpty(cssClass)) {
                cssClass = "active";
            }

            string path = html.ViewContext.RequestContext.HttpContext.Request.Path;

            if (AppService.Current.Navigation.IsMatched(path, navKey)) {
                return cssClass;
            } else {
                return string.Empty;
            }

        }

        public static string IsSelected(this HtmlHelper html, string controller = null, string action = null, string cssClass = null) {

            string currentAction = (string)html.ViewContext.RouteData.Values["action"];
            string currentController = (string)html.ViewContext.RouteData.Values["controller"];

            if (String.IsNullOrEmpty(cssClass)) {
                cssClass = "active";
            }

            if (String.IsNullOrEmpty(controller)) {
                controller = currentController;
            }

            if (String.IsNullOrEmpty(action)) {
                action = currentAction;
            }


            return controller == currentController && action == currentAction ?
                cssClass : String.Empty;
        }

        public static string PageClass(this HtmlHelper html) {
            string currentAction = (string)html.ViewContext.RouteData.Values["action"];
            return currentAction;
        }
    }
}