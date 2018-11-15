using AABC.DomainServices.Patients;
using AABC.PatientPortal.App.Account.Models;
using System;
using System.Web.Mvc;
using WebMatrix.WebData;
namespace AABC.PatientPortal.App.Account
{

    [Authorize]    
    public class AccountController : Controller
    {
        SignInService svcSignIn;
        public AccountController()
        {
            svcSignIn = new SignInService(AppService.Current.Data.Context);
        }

        [AllowAnonymous]
        public ActionResult Login(string returnUrl) {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }


        [HttpPost]
        [AllowAnonymous]
        public ActionResult Login(LoginVM model, string returnUrl) {

            if (ModelState.IsValid) {

                if (WebSecurity.Login(model.UserName, model.Password, persistCookie: model.RememberMe)) {
                    int UserId = svcSignIn.GetUserId(model.UserName);
                    svcSignIn.Save(new SignInEx() { UserId = UserId, SignInDate = DateTime.Now, SignInType = "Login" });
                    return Redirect(returnUrl ?? "/");
                } else {
                    ModelState.AddModelError("", "Invalid username or password.");
                }
            }

            return View(model);
        }

        public ActionResult Logout() {
            CacheService.Current.User.RemoveAll();
            WebSecurity.Logout();
            return RedirectToAction("Login", "Account");
        }


    }
}