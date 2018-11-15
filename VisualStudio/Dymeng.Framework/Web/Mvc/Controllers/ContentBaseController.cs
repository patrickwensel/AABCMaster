using Dymeng.Framework.Web.Mvc.Views;
using System;
using System.Web.Mvc;


namespace Dymeng.Framework.Web.Mvc.Controllers
{

    [Authorize]
    public abstract class ContentBaseController : Controller
    {


        public ActionResult TransferToAction(string action) {
            string routeUrl = "/" + this.ControllerContext.RouteData.Values["controller"].ToString() + "/" + action;
            return new TransferResult(routeUrl);
        }

        public ActionResult TransferToAction(string action, string controller) {
            return new TransferResult("/" + controller + "/" + action);
        }
        

        public bool PushState {
            get
            {

                if (Request == null) {
                    System.Diagnostics.Debug.WriteLine("REQUEST: null");
                    return false;
                }
                
                if (Request.IsAjaxRequest() && Request.Params["navigateBack"] == null) {
                    return true;
                } else {
                    return false;
                }
            }
        }



        protected ActionResult SaveAction(object model, IWebViewHelper viewHelper, string successRedirectActionName, string validationFailureRedirectActionName, string errorRedirectActionName, Action saveMethod) {

            viewHelper.BindModel();

            if (!viewHelper.Validate()) {
                viewHelper.ReturnErrorMessage = "Please validate all fields before saving.";
                return View(validationFailureRedirectActionName, model);
            }
            try {
                saveMethod();
                return RedirectToAction(successRedirectActionName);
            }
            catch {
                viewHelper.ReturnErrorMessage = "We encountered an unexpected problem while saving.  Please try again or contact your administrator if the problem persists.";
                return PartialView(errorRedirectActionName, model);
            }
        }


        protected ActionResult SaveFullAction(object model, IWebViewHelper viewHelper, string successRedirectActionName, string validationFailureRedirectActionName, string errorRedirectActionName, Action saveMethod)
        {

            viewHelper.BindModel();

            if (!viewHelper.Validate())
            {
                if (String.IsNullOrEmpty(viewHelper.ReturnErrorMessage))
                {
                    viewHelper.ReturnErrorMessage = "Please validate all fields before saving.";
                }
                return GetView(validationFailureRedirectActionName, model);
            }
            try
            {
                saveMethod();
                return RedirectToAction(successRedirectActionName);
            }
            catch
            {
                viewHelper.ReturnErrorMessage = "We encountered an unexpected problem while saving.  Please try again or contact your administrator if the problem persists.";
                return PartialView(errorRedirectActionName, model);
            }
        }

        protected ActionResult SaveAction(object model, IWebViewHelper viewHelper, Action saveMethod) {
            return SaveAction(model, viewHelper, "Index", "Index", "ErrorPartial", saveMethod);
        }


        protected ActionResult GetView() {
            ViewBag.PushState = this.PushState;
            if (Request.IsAjaxRequest()) {
                return PartialView();
            } else {
                return View();
            }
        }
                
        protected ActionResult GetView(object model) {
            ViewBag.PushState = this.PushState;
            if (Request.IsAjaxRequest()) {
                return PartialView(model);
            } else {
                return View(model);
            }
        }

        protected ActionResult GetView(string view) {
            ViewBag.PushState = this.PushState;
            if (Request.IsAjaxRequest()) {
                return PartialView(view);
            } else {
                return View(view);
            }
        }

        protected ActionResult GetView(string view, object model) {
            ViewBag.PushState = this.PushState;

            if (Request == null) {
                return View(view, model);
            }

            if (Request.IsAjaxRequest()) {
                return PartialView(view, model);
            } else {
                return View(view, model);
            }
        }

        protected ActionResult GetViewOrGridCallback(string coreViewName, string gridViewName, object model) {

            bool isFilterCallback = false;
            try {
                isFilterCallback = Request.Params["isFilterCallback"] == "true";
            } catch {
                // bury
            }
            

            if (isFilterCallback) {
                return GetView(gridViewName, model);
            } else {
                return GetView(coreViewName, model);
            }

        }

        protected ActionResult GetViewOrGridCallback(string coreViewName, string gridViewName, string paramCallbackIndicatorName, object model) {

            bool isFilterCallback = Request.Params[paramCallbackIndicatorName] == "true";

            if (isFilterCallback) {
                return GetView(gridViewName, model);
            } else {
                return GetView(coreViewName, model);
            }

        }



    }
}
