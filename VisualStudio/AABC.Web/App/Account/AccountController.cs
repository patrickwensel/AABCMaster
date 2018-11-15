using AABC.Web.App.Account.Models;
using AABC.Web.Filters;
using System;
using System.Web.Mvc;
using System.Web.Security;
using WebMatrix.WebData;

namespace AABC.Web.App.Account
{

    [Authorize]
    [InitializeSimpleMembership]
    public class AccountController : Controller
    {

        /**********************
         *  POSTS
         * *******************/
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                if (WebSecurity.Login(model.UserName, model.Password, persistCookie: model.RememberMe.Value))
                {

                    Data.Services.WebUserService.InvalidateUserCache(model.UserName);

                    return Redirect(returnUrl ?? "/");
                }
                ModelState.AddModelError("", "The user name or password provided is incorrect.");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterModel model)
        {

            if (!Global.Default.User().HasPermission(Domain.Admin.Permissions.UserManagement))
            {
                throw new UnauthorizedAccessException();
            }

            try
            {
                WebSecurity.InitializeDatabaseConnection("DefaultConnection", "UserProfile", "UserId", "UserName", autoCreateTables: true);
            }
            catch (Exception)
            {
                // skip "initializedatabaseconnection can only be called once...
                // TODO: find out exact exception so we're not burying everything
            }

            WebSecurity.CreateUserAndAccount(model.UserName, model.Password);
            //System.Diagnostics.Debug.WriteLine("TEMP PASS: " + model.Password);
            return new EmptyResult();

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword(ChangePasswordModel model)
        {
            if (ModelState.IsValid)
            {
                bool changePasswordSucceeded;
                try
                {
                    changePasswordSucceeded = WebSecurity.ChangePassword(User.Identity.Name, model.OldPassword, model.NewPassword);
                }
                catch (Exception)
                {
                    changePasswordSucceeded = false;
                }
                if (changePasswordSucceeded)
                {
                    return RedirectToAction("ChangePasswordSuccess");
                }
                else
                {
                    ModelState.AddModelError("", "The current password is incorrect or the new password is invalid.");
                }

            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        /**********************
         *  GETS
         * *******************/
        [AllowAnonymous]
        public ActionResult Login()
        {
            return View();
        }

        public ActionResult LogOff()
        {
            Data.Services.WebUserService.InvalidateUserCache(User.Identity.Name);
            WebSecurity.Logout();

            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        public ActionResult Register()
        {
            if (!Global.Default.User().HasPermission(Domain.Admin.Permissions.UserManagement))
            {
                throw new UnauthorizedAccessException();
            }
            return View();
        }

        public ActionResult ChangePassword()
        {
            return View();
        }

        /**********************
         *  DELETES
         * *******************/
        [HttpPost]
        [Authorize]
        public ActionResult Delete(string userName)
        {

            if (!Global.Default.User().HasPermission(Domain.Admin.Permissions.UserManagement))
            {
                throw new UnauthorizedAccessException();
            }

            try
            {
                ((SimpleMembershipProvider)Membership.Provider).DeleteAccount(userName);
                ((SimpleMembershipProvider)Membership.Provider).DeleteUser(userName, true);

                return View();
            }
            catch
            {
                return View();
            }
        }


        /**********************
         *  DEVEX/CALLBACKS
         * *******************/



        /**********************
         *  HELPERS
         * *******************/



        /**********************
         *  UNKNOWN
         * *******************/



        /**********************
         *  CONTROLLER SETUP
         * *******************/


        //public string RegisterAccount(RegisterModel model) {

        //    //if (ModelState.IsValid) {

        //    //	var result = WebSecurity.CreateUserAndAccount(model.UserName, model.Password);
        //    //	if (!string.IsNullOrEmpty(result)) {
        //    //		int userId = WebSecurity.GetUserId(model.UserName);

        //    //		return "1";
        //    //	}
        //    //}

        //    return null;
        //}


        public ActionResult ChangePasswordSuccess()
        {
            return View();
        }

        public int GetUserID(string username)
        {
            return WebSecurity.GetUserId(username);
        }

        #region Status Codes
        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // See http://go.microsoft.com/fwlink/?LinkID=177550 for
            // a full list of status codes.
            switch (createStatus)
            {
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