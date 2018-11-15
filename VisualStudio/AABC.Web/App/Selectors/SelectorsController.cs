using System;
using System.Web.Mvc;
namespace AABC.Web.Controllers
{
    public class SelectorsController : Controller
    {
        
        
        Models.Shared.ZipLookupVM getZipLookupModel(string state = null) {

            var zipService = new Data.Services.ZipCodeService();

            var model = new Models.Shared.ZipLookupVM();

            model.zluStatesList = zipService.GetStates();
            model.zluCitiesList = zipService.GetCitiesList(state);
            model.zluCountiesList = zipService.GetCountiesList(state);
            
            return model;
            
        }


        public ActionResult ZipsByStateAndCity(string state, string city) {

            var zipService = new Data.Services.ZipCodeService();

            var items = zipService.GetZipsByCity(state, city);

            if (items == null || items.Count == 0) {
                return Content("");
            } else {
                return Content(string.Join(", ", items.ToArray()));
            }

        }

        public ActionResult ZipsByStateAndCounty(string state, string county) {

            var zipService = new Data.Services.ZipCodeService();

            var items = zipService.GetZipsByCounty(state, county);

            if (items == null || items.Count == 0) {
                return Content("");
            } else {
                return Content(string.Join(", ", items.ToArray()));
            }

        }


        public ActionResult ZipLookup() {
            
            return PartialView("ZipLookupMain", getZipLookupModel());
        }

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult ZipLookupStateCallback() {
            
            var model = getZipLookupModel();
            return PartialView("ZipLookupState", model);
        }

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult ZipLookupCityCallback() {

            var zipService = new Data.Services.ZipCodeService();

            var model = getZipLookupModel(Request.Params["SelectedState"]);
            
            return PartialView("ZipLookupCity", model);
        }


        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult ZipLookupCountyCallback() {

            var zipService = new Data.Services.ZipCodeService();

            var model = getZipLookupModel(Request.Params["SelectedState"]);
            
            return PartialView("ZipLookupCounty", model);
        }













        public ActionResult GetDate(DateTime? existingDate) {

            var model = new Models.Shared.DateSelectorVM();
            model.ExistingDate = existingDate;

            return PartialView("GetDatePopup", model);
            
        }

    }
}