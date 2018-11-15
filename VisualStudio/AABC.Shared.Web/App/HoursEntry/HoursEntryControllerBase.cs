using AABC.Domain2.Providers;
using AABC.DomainServices.HoursResolution;
using AABC.Shared.Web.App.HoursEntry.Models.Request;
using System;
using System.Linq;
using System.Web.Mvc;

namespace AABC.Shared.Web.App.HoursEntry
{
    public abstract class HoursEntryControllerBase<TService> : Controller
        where TService : HoursEntryServiceBase
    {

        protected TService Service { get; private set; }
        protected EntryApp EntryApp { get; private set; }

        public HoursEntryControllerBase(TService service, EntryApp entryApp = EntryApp.Unknown)
        {
            Service = service;
            EntryApp = entryApp;
        }


        [HttpPost]
        public bool Delete(int hoursID)
        {
            Service.DeleteHours(hoursID);
            return true;
        }

        [HttpGet]
        public JsonResult GetServices(int caseID, int providerID, DateTime date)
        {
            var services = Service.GetServices(caseID, providerID, date);
            return Json(services, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetActivePatients(int providerID, DateTime date)
        {
            var model = Service.GetHoursEntryActivePatients(providerID, date);
            return Json(new { patients = model }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetActiveInsurance(int patientID, DateTime date)
        {
            var insurance = Service.GetActiveInsurance(patientID, date);
            return Json(insurance != null ? new { insuranceID = insurance.ID, insuranceName = insurance.Name } : null, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public string GetDeleteConfirmSummary(int hoursID)
        {
            var model = Service.GetDeleteConfirmSummary(hoursID);
            return model;
        }


        [HttpGet]
        public JsonResult GetEntryVM(int hoursID, int? editingByProviderID)
        {
            var model = Service.GetHoursEntryVM(hoursID, editingByProviderID);
            return new DateFormattedJsonResult { Data = model };  // Json(model, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public JsonResult GetSessionReportConfig(ProviderTypeIDs providerTypeID, int serviceID)
        {
            var model = Service.GetSessionReportConfig(providerTypeID, serviceID);
            return this.CamelCaseJson(model, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public ActionResult ProviderSelect(int caseID)
        {
            var model = Service.GetProviderSelectModel(caseID);
            return PartialView("ProviderSelectPopup", model.ToList());
        }

        [HttpGet]
        public JsonResult GetNewEntryDataTemplate(int caseID, int providerID, DateTime? date)
        {
            var model = Service.GetNewHoursEntryDataTemplate(caseID, providerID, date);
            return new DateFormattedJsonResult { Data = model };
        }


        [HttpPost]
        public ActionResult Submit(bool isOnAideLegacyMode)
        {
            var requestModel = GetModel(isOnAideLegacyMode);
            var response = Service.SubmitHoursRequest(requestModel, EntryApp, isOnAideLegacyMode);
            return Json(response);
        }


        [HttpPost]
        public ActionResult Validate(bool isOnAideLegacyMode)
        {
            var requestModel = GetModel(isOnAideLegacyMode);
            var response = Service.SubmitHoursForValidation(requestModel, EntryApp, isOnAideLegacyMode);
            return Json(response);
        }


        private BaseHoursEntryRequestVM GetModel(bool isOnAideLegacyMode)
        {
            if (isOnAideLegacyMode)
            {
                var model = new HoursEntryRequestVM();
                UpdateModel(model);
                return model;
            }
            else
            {
                var model = new HoursEntryRequest2VM();
                UpdateModel(model);
                return model;
            }
        }
    }
}
