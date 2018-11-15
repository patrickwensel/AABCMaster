using AABC.ProviderPortal.Filters;
using AABC.ProviderPortal.Models;
using System;
using System.Web.Mvc;
using System.Web.Security;
using WebMatrix.WebData;

namespace AABC.ProviderPortal.Controllers
{
    [Authorize]
    [InitializeSimpleMembership]
    public class AccountController : Controller {

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login() {

            //WebSecurity.CreateUserAndAccount("reb6", "password");

            return View();
        }

        //
        // POST: /Account/Login

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel model, string returnUrl) {

            if(ModelState.IsValid) {
                if(WebSecurity.Login(model.UserName, model.Password, persistCookie: model.RememberMe.Value)) {
                    Global.ClearInstance();
                    return Redirect(returnUrl ?? "/");
                } else {
                    ModelState.AddModelError("", "The user name or password provided is incorrect.");
                    ViewBag.LoginError = true;
                }
                
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [AllowAnonymous]
        public ActionResult Impersonate()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Impersonate(ImpersonateModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.UserName == "admin" && WebSecurity.Login(model.UserName, model.Password))
                {
                    Global.ClearInstance();
                    FormsAuthentication.SetAuthCookie(model.ImpersonateUser, false);
                    return Redirect("/");
                }
                else
                {
                    ModelState.AddModelError("", "The user name or password provided is incorrect.");
                    ViewBag.LoginError = true;
                }

            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/LogOff

        public ActionResult LogOff() {
            Global.ClearInstance();
            WebSecurity.Logout();
            return RedirectToAction("Login", "Account");
        }

        //
        // GET: /Account/Register

        [Authorize]
        public ActionResult Register() {
            return View();
        }

        //
        // POST: /Account/Register

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterModel model) {
            if(ModelState.IsValid) {
                // Attempt to register the user
                            try {
                    WebSecurity.CreateUserAndAccount(model.UserName, model.Password);
                    WebSecurity.Login(model.UserName, model.Password);
                    return RedirectToAction("Index", "Home");
                }
                catch(MembershipCreateUserException e) {
                    ModelState.AddModelError("", ErrorCodeToString(e.StatusCode));
                }
                        }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

		public string RegisterAccount(RegisterModel model) {

			//if (ModelState.IsValid) {

			//	Console.Write("Testing");

			//	var result = WebSecurity.CreateUserAndAccount(model.UserName, model.Password);
			//	if (!string.IsNullOrEmpty(result)) {
			//		int userId = WebSecurity.GetUserId(model.UserName);

			//		return "1";
			//	}
			//}

			return null;
		}

		//
		// GET: /Account/ChangePassword

		public ActionResult ChangePassword() {
            return View();
        }

        //
        // POST: /Account/ChangePassword

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword(ChangePasswordModel model) {
            if(ModelState.IsValid) {
                bool changePasswordSucceeded;
                try {
                    changePasswordSucceeded = WebSecurity.ChangePassword(User.Identity.Name, model.OldPassword, model.NewPassword);
                }
                catch(Exception) {
                    changePasswordSucceeded = false;
                }
                if(changePasswordSucceeded) {
                    return Content("OK");
                }
                else {
					return Content("ERR: The current password is incorrect or the new password is invalid.");
				}
                
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ChangePasswordSuccess

        public ActionResult ChangePasswordSuccess() {
            return View();
        }

		[HttpGet]
		public ActionResult UserChangePassword() {

			return PartialView();

		}

		[HttpPost]
		public ActionResult UserChangePassword(ChangePasswordModel model) {

			if (ModelState.IsValid) {
				bool changePasswordSucceeded;
				try {
					changePasswordSucceeded = WebSecurity.ChangePassword(User.Identity.Name, model.OldPassword, model.NewPassword);
				}
				catch (Exception) {
					changePasswordSucceeded = false;
				}
				if (changePasswordSucceeded) {
					return Content("OK");
				}
				else {
					return Content("ERR: The current password is incorrect or the new password is invalid.");
				}

			}

			return View(model);

		}

		#region Status Codes
		private static string ErrorCodeToString(MembershipCreateStatus createStatus) {
            // See http://go.microsoft.com/fwlink/?LinkID=177550 for
            // a full list of status codes.
            switch(createStatus) {
                case MembershipCreateStatus.DuplicateUserName:
                    return "User name already exists. Please enter a different user name.";

                case MembershipCreateStatus.DuplicateEmail:
                    return "A user name for that e-mail address already exists. Please enter a different e-mail address.";

                case MembershipCreateStatus.InvalidPassword:
                    return "The password provided is invalid. Please enter a valid password value.";

                case MembershipCreateStatus.InvalidEmail:
                    return "The e-mail address provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "The password retrieval answer provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "The password retrieval question provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidUserName:
                    return "The user name provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.ProviderError:
                    return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                case MembershipCreateStatus.UserRejected:
                    return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                default:
                    return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
            }
        }
        #endregion
    }
}