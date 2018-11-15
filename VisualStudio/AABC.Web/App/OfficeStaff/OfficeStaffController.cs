using Dymeng.Framework.Web.Mvc.Views;
using System.Web.Mvc;

namespace AABC.Web.Controllers
{

    [Authorize]
    public class OfficeStaffController : Dymeng.Framework.Web.Mvc.Controllers.ContentBaseController
    {

        
        /**********************
         *  POSTS
         * *******************/
        [HttpPost]
        public ActionResult Edit(Models.OfficeStaff.OfficeStaffViewModel model) {
            return SaveAction(model, model.ViewHelper, "Search", "Edit", "ErrorPartial", () => repository.SaveStaff(model.Detail));
        }

        


        /**********************
         *  GETS
         * *******************/

        [HttpGet]
        public ActionResult Index() {
            return RedirectToAction("Search");
        }
        
        [HttpGet]
        public ActionResult Search() {

            var model = new Models.OfficeStaff.OfficeStaffListModel();
            model.DetailList = repository.GetOfficeStaffList();
            model.Base = new ViewModelBase(PushState, "/OfficeStaff/Search", "Office Staff Search");

            model.ListBase.GridTitlePanelSettings = new GridTitlePanelSettings("Office Staff", true);
            model.ListBase.GridTitlePanelSettings.AddNewAction = "Create";
            model.ListBase.GridTitlePanelSettings.AddNewController = "OfficeStaff";

            bool isFilterCallback = Request.Params["isFilterCallback"] == "true";

            if (isFilterCallback) {
                return GetView("ListGrid", model);
            } else {
                return GetView("List", model);
            }
        }


        [HttpGet]
        public ActionResult Create() {

            var model = new Models.OfficeStaff.OfficeStaffViewModel();
            model.Base = new ViewModelBase(PushState, "/OfficeStaff/Create", "New Office Staff Entry");

            return GetView("Edit", model);
        }

        [HttpGet]
        public ActionResult Edit(int id) {

            var model = new Models.OfficeStaff.OfficeStaffViewModel();
            model.Detail = repository.GetOfficeStaff(id);
            model.Base = new ViewModelBase(
                PushState,
                "/OfficeStaff/Edit/" + id,
                "Office Staff Editing",
                "/OfficeStaff/Search");

            return GetView("Edit", model);
        }


        /**********************
         *  DELETES
         * *******************/
        [HttpPost]
        public ActionResult Delete(int id) {
            Models.OfficeStaff.OfficeStaffHelpers.DeleteStaff(id);
            return RedirectToAction("Search");
        }


        /**********************
         *  DEVEX CALLBACKS
         * *******************/
        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult SearchGridFilter() {
            return Search();
        }


        /**********************
         *  HELPERS
         * *******************/
        public ActionResult ConfirmDelete(int id) {

            string message =
                string.Format(
                    "<p><span style='font-weight: 600;'>Warning!</span> Are you sure you want to remove Office Staff member {0} from the system?</p> " +
                    "<p>Any associated data and assignments will be lost.</p>",
                    Models.OfficeStaff.OfficeStaffHelpers.GetStaffCommonName(id)
                );

            Models.Shared.DialogConfirmVM model = new Models.Shared.DialogConfirmVM(message);
            model.Default = Models.Shared.DialogConfirmVM.Choices.Button2;
            model.Icon = Models.Shared.DialogIcon.Warning;

            return PartialView("DialogConfirmPartialView", model);
        }

        /**********************
         *  CONTROLLER SETUP
         * *******************/
        private Repositories.IOfficeStaffRepository repository;

        public OfficeStaffController() {
            repository = new Repositories.OfficeStaffRepository();
        }

        public OfficeStaffController(Repositories.IOfficeStaffRepository repository) {
            this.repository = repository;
        }
        

    }
}
