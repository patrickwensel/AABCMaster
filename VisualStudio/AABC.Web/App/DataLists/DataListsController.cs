using System.Web.Mvc;

namespace AABC.Web.Controllers
{
    public class DataListsController : Controller
    {


        /**********************
         *  POSTS
         * *******************/
        [HttpPost]
        public ActionResult AuthDetailEdit(int authID, string authCode, string authDescription) {
            repository.EditAuthDetail(authID, authCode, authDescription);
            return Content("ok");   
        }

        [HttpPost]
        public ActionResult AuthDetailCreate(string authCode, string authDescription) {
            repository.CreateAuthDetail(authCode, authDescription);
            return Content("ok");
        }

        [HttpPost]
        public ActionResult AuthDetailDelete(int id) {
            if (!repository.DeleteAuthDetail(id)) {
                return Content("dependency");
            }
            return Content("ok");
        }



        /**********************
         *  GETS
         * *******************/


        public ActionResult Index() {
            return PartialView("Index");
        }

        public ActionResult AuthDetail(int? authID) {

            var model = new Models.DataLists.AuthDetailVM();

            if (authID != null) {
                ViewBag.AuthID = authID;
                model = repository.GetAuthDetail(authID.Value);
            }
            
            return PartialView("AuthDetail", model);
        }



        /**********************
         *  DELETES
         * *******************/



        /**********************
         *  DEVEX/CALLBACKS
         * *******************/

        public ActionResult AuthsGrid() {
            var model = new Models.DataLists.AuthGridVM();
            model.Items = repository.GetAuthGridListItems();
            return PartialView("AuthsGrid", model);
        }


        /**********************
         *  HELPERS
         * *******************/



        /**********************
         *  UNKNOWN
         * *******************/



        /**********************
         *  CONTROLLER SETUP
         * *******************/

        Repos.DataListsRepo repository;

        public DataListsController() {
            repository = new Repos.DataListsRepo();
        }





    }
}