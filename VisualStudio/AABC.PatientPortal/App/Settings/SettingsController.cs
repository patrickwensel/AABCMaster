using System;
using System.Web.Mvc;

namespace AABC.PatientPortal.App.Settings
{
    [Authorize]
    public class SettingsController : Dymeng.Web.Mvc.ControllerBase
    {

        public ActionResult Index() {

            var model = new Models.SettingsVM();

            model.UserID = AppService.Current.User.ID;

            model.DisplayNameVM = new Models.DisplayNameVM();
            model.DisplayNameVM.FirstName = AppService.Current.User.CurrentUser.FirstName;
            model.DisplayNameVM.LastName = AppService.Current.User.CurrentUser.LastName;

            model.Signature = AppService.Current.User.CurrentUser.GetCurrentSignature()?.Data;

            return View("Index", model);
        }



        [HttpPost]
        public ActionResult Signature(string data) {

            var context = AppService.Current.Data.Context;
            var sig = new Domain2.PatientPortal.ParentSignature();
            sig.Data = data;
            sig.Date = DateTime.Now;
            sig.LoginID = AppService.Current.User.ID;

            context.ParentSignatures.Add(sig);

            context.SaveChanges();
            AppService.Current.User.ClearCache();

            return Content("ok");

        }

        [HttpPost]
        public ActionResult DisplayName(Models.DisplayNameVM model) {

            try {
                _service.UpdateDisplayName(model);
                return Content("ok");
            } catch {
                return Content("err");
            }
            
        }

        [HttpPost]
        public ActionResult ChangePassword(Models.ChangePasswordVM model) {

            if (!_service.CheckPassword(model.OldPassword)) {
                return Content("Incorrect password.  Please try again.");
            }

            if (model.NewPassword != model.ConfirmPassword) {
                return Content("New Password and confirmation don't match.  Please try again.");
            }

            if (model.NewPassword == null || string.IsNullOrEmpty(model.NewPassword) || string.IsNullOrWhiteSpace(model.NewPassword)) {
                return Content("New password isn't long/strong enough.  Please try again.");
            }

            if (model.NewPassword.Length < 6) {
                return Content("New password should be at least 6 characters long.  Please try again.");
            }
                               

            _service.UpdatePassword(model);

            return Content("ok");

        }





        private SettingsService _service;
        public SettingsController() {
            _service = new SettingsService();
        }
    }
}