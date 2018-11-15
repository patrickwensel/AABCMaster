using System.Web.Mvc;

namespace AABC.ProviderPortal.Controllers
{
    public class DialogsController : Controller
    {
        

        public ActionResult PopupGeneral(Models.Shared.PopupGeneralOptions options) {

            var model = new Models.Shared.PopupGeneralViewModel();

            model.Height = options.height.HasValue ? options.height.Value : model.Height;
            model.Width = options.width.HasValue ? options.width.Value : model.Width;
            model.Title = options.title != null ? options.title : model.Title;
            model.AllowDrag = options.draggable.HasValue ? options.draggable.Value : model.AllowDrag;
            

            return PartialView("PopupGeneral", model);

        }



        public ActionResult ConfirmDeleteV2(string message) {

            var model = new Models.Shared.DialogConfirmModel(message);
            model.Default = Models.Shared.DialogConfirmModel.Choices.Button2;
            model.Icon = Models.Shared.DialogIcon.Warning;
            model.DialogCloseScript = null;
            model.ResultVariableName = "App.Internal.DialogResults.ConfirmResult";

            return PartialView("DialogConfirmPartialView", model);

        }

        public ActionResult ConfirmDelete(string contentElementID, string message) {
            // DEPRECATED, use ConfirmDeleteV2 Instead
            // (see js.App.Dialogs.ConfirmDelete for usage
            Models.Shared.DialogConfirmModel model = new Models.Shared.DialogConfirmModel(message);
            model.Default = Models.Shared.DialogConfirmModel.Choices.Button2;
            model.Icon = Models.Shared.DialogIcon.Warning;

            return PartialView("DialogConfirmPartialView", model);

        }


        public ActionResult MessageBox(string message, int? icon, string title) {
            var model = new Models.Shared.MessageBoxModel();
            model.Icon = Models.Shared.DialogIcon.None;
            model.Title = title ?? "";
            if (icon.HasValue) {
                try {
                    model.Icon = (Models.Shared.DialogIcon)icon.Value;
                }
                catch {
                    // leave blank
                }
            }
            model.Message = message;
            return PartialView("MessageBox", model);
        }

        public ActionResult Error(string message) {
            var model = new Models.Shared.MessageBoxModel();
            model.Message = message;
            model.Icon = Models.Shared.DialogIcon.Critical;
            model.Title = "Error";
            return PartialView("ErrorPopup", model);
        }

    }
}