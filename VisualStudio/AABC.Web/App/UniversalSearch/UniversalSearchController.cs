using System.Web.Mvc;

namespace AABC.Web.Controllers
{
    [Authorize]
    public class UniversalSearchController : Controller
    {

        /**********************
         *  POSTS
         * *******************/
        
        [HttpPost]
        public ActionResult Search(string type, string term) {

            switch (type) {

                case "patient":
                    return RedirectToAction("SearchPatients", new { term = term });

                case "provider":
                    return RedirectToAction("SearchProviders", new { term = term });

                case "referral":
                    return RedirectToAction("SearchReferrals", new { term = term });
                    
                default:
                    return Content("ERR: The specified search type is not recognized.");
            }
        }


        /**********************
         *  GETS
         * *******************/
        
        [HttpGet]
        public ActionResult Search() {
            return PartialView("Search");
        }

        [HttpGet]
        public ActionResult SearchPatients(string term) {
            var vm = new Models.UniversalSearch.FirstLastNameResultGridVM();
            vm.Items = repository.GetPatientSearchResult(term);
            return PartialView("FirstLastNameResultGrid", vm);
        }

        [HttpGet]
        public ActionResult SearchProviders(string term) {
            var vm = new Models.UniversalSearch.FirstLastNameResultGridVM();
            vm.Items = repository.GetProviderSearchResult(term);
            return PartialView("FirstLastNameResultGrid", vm);
        }

        [HttpGet]
        public ActionResult SearchReferrals(string term) {
            var vm = new Models.UniversalSearch.FirstLastNameResultGridVM();
            vm.Items = repository.GetReferralSearchResult(term);
            return PartialView("FirstLastNameResultGrid", vm);
        }

        /**********************
         *  DELETES
         * *******************/



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

        Repositories.UniversalSearchRepository repository = new Repositories.UniversalSearchRepository();





    }
}