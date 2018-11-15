using AABC.Web.Infrastructure;

namespace System.Web.Mvc
{
    public static class ControllerExtensions
    {
        public static JsonResult CamelCaseJson(this Controller controller, object data)
        {
            return new JsonDotNetResult
            {
                Data = data,
                JsonRequestBehavior = JsonRequestBehavior.DenyGet
            };
        }

        public static JsonResult CamelCaseJson(this Controller controller, object data, JsonRequestBehavior behavior)
        {
            return new JsonDotNetResult
            {
                Data = data,
                JsonRequestBehavior = behavior
            };
        }
    }
}