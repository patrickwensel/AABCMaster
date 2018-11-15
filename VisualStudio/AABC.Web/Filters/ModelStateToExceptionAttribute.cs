using Dymeng.Framework.Web.Mvc.Controllers;
using System;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace AABC.Web.Filters
{
    public class ModelStateToExceptionAttribute : ActionFilterAttribute
    {

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var ms = filterContext.Controller.ViewData.ModelState;
            if (!ms.IsValid)
            {
                var errors = ModelStateHelper.GetAllModelStateErrors(ms);
                var action = filterContext.ActionDescriptor.ActionName;
                var controller = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;
                var user = filterContext.HttpContext.User.Identity.Name;
                var s = ModelStateHelper.FormatModelStateErrorsForLogging(errors, action, controller, user);
                var ex = new ArgumentException(s);
                Task.Factory.StartNew(() =>
                {
                    Dymeng.Framework.Exceptions.Handle(ex);
                });
                throw ex;
            }
        }
    }
}