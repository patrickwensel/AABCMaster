using DevExpress.Web.Mvc;

namespace AABC.Web.Helpers
{
    public static class NavBarExtensions
    {
        /// <summary>
        /// Adds a menu subitem to the group.
        /// </summary>
        public static void AddMenuItem(this MVCxNavBarGroup group, string text, string controller, string action)
        {
            group.Items.Add(text, action, null, DevExpressHelper.GetUrl(new { Controller = controller, Action = action }));
        }
    }
}