using System;
using System.Web.Mvc;

namespace AABC.ProviderPortal.Controllers
{
    public class SelectorsController : Controller
    {
        
        public ActionResult GetDate(DateTime? existingDate) {

            var model = new Models.Shared.GetDateSelectorModel();
            model.ExistingDate = existingDate;

            return PartialView("GetDatePopup", model);
            
        }

    }
}