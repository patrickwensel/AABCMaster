using AABC.DomainServices.HoursResolution;
using System;
using System.Web.Mvc;

namespace AABC.ProviderPortal.Controllers
{
    public class HoursEntryController : Shared.Web.App.HoursEntry.HoursEntryControllerBase<App.HoursEntry.HoursEntryService>
    {

        public HoursEntryController() : base(new App.HoursEntry.HoursEntryService(new Data.V2.CoreContext()), EntryApp.ProviderPortal)
        {
        }

        public ActionResult GetEntryHtml()
        {
            return PartialView("HoursEntry");
        }

        [HttpGet]
        public JsonResult GetEntryOrPreEntryVM(int hoursID, int? editingByProviderID)
        {
            if (hoursID < 0)
            {
                // this is a prefilled catalyst entry
                var model = Service.GetCatalsytHoursVM(Math.Abs(hoursID));
                return new DateFormattedJsonResult() { Data = model };  // Json(model, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return GetEntryVM(hoursID, editingByProviderID);
            }
        }

    }
}