using AABC.Domain2.Providers;
using AABC.DomainServices.Cases;
using AABC.Web.App.Staffing.Models;
using AABC.Web.Infrastructure.JsonResult;
using DevExpress.Web.Mvc;
using Dymeng.Framework.Web.Mvc.Views;
using System.Collections.Generic;
using System.Net;
using System.Web.Mvc;

namespace AABC.Web.App.Staffing
{
    public class StaffingController : Dymeng.Framework.Web.Mvc.Controllers.ContentBaseController
    {

        public ActionResult Search(bool isFilterCallback = false)
        {
            var title = "Staffing - Manage";
            var model = new StaffingListVM(ListViewType.Active)
            {
                DetailList = service.GetStaffingLogListItems(),
                Base = new ViewModelBase(PushState, "/Staffing/Search", title)
            };
            model.ListBase.GridTitlePanelSettings = new GridTitlePanelSettings(title, false);
            if (isFilterCallback)
            {
                return GetView("ListGrid", model);
            }
            else
            {
                return GetView("List", model);
            }
        }


        [HttpGet]
        public ActionResult Manage(int id)
        {
            ViewBag.CaseID = id;
            return View("StaffingManager");
        }


        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult GetTab(int tabIndex, int caseID)
        {
            switch (tabIndex)
            {
                case 1:
                    return Providers(caseID);
                case 2:
                    return Timeline(caseID);
                default:
                    return Summary(caseID);
            }
        }


        private ActionResult Summary(int caseID)
        {
            var staffingLog = repository.GetByID(caseID);
            //if (staffingLog == null)
            //{
            //    staffingLog = repository.CreateStaffingLog(caseID);
            //}
            var model = service.CreateStaffingLogVM(caseID, staffingLog);
            model.Base = new ViewModelBase(PushState, "/Staffing/Edit/" + caseID, "Staffing/Case Edit", "/Staffing/Search");
            return GetView("SummaryTab", model);
        }


        public ActionResult Providers(int caseID)
        {
            var model = new ProvidersTabVM
            {
                CaseID = caseID,
                Base = new ViewModelBase(PushState, "/Staffing/Providers", "Provider Search", "/Staffing/Search")
            };
            return GetView("ProvidersTab", model);
        }


        private ActionResult Timeline(int caseId)
        {
            var model = new ProvidersTabVM
            {
                CaseID = caseId,
                Base = new ViewModelBase(PushState, "/Staffing/Providers", "Provider Search", "/Staffing/Search")
            };
            return GetView("TimelineTab", model);
        }


        public ActionResult SelectableProviders(int caseID)
        {
            var model = new ProvidersListVM<ProviderListItemVM>
            {
                CaseID = caseID,
                Providers = repository.GetSelectableProviders(caseID)
            };
            model.ListBase.GridTitlePanelSettings.Title = "Select Providers";
            var staffingLog = repository.GetByID(caseID);
            model.PreferredGender = staffingLog.ProviderGenderPreference;
            return GetView("ProvidersGrid", model);
        }


        public ActionResult SelectedProviders(int caseID)
        {
            var model = new ProvidersListVM<SelectedProviderListItemVM>
            {
                CaseID = caseID,
                Providers = repository.GetSelectedProviders(caseID)
            };
            model.ListBase.GridTitlePanelSettings.Title = "Selected Providers";
            return GetView("SelectedProvidersGrid", model);
        }


        [HttpPost]
        public ActionResult Edit(StaffingLogVM model)
        {
            var request = new StaffingLogSaveRequest
            {
                ID = model.CaseID,
                ParentalRestaffRequest = model.ParentalRestaffRequest,
                HoursOfABATherapy = model.HoursOfABATherapy,
                AidesRespondingNo = model.AidesRespondingNo,
                AidesRespondingMaybe = model.AidesRespondingMaybe,
                ScheduleRequest = model.ScheduleRequest != null ? model.ScheduleRequest.ToInt() : 0,
                SpecialAttentionNeedIds = CheckBoxListExtension.GetSelectedValues<int>("SpecialAttentionNeedIds"),
                ProviderGenderPreference = model.ProviderGenderPreference != '0' ? model.ProviderGenderPreference : null
            };
            return SaveFullAction(model, model.ViewHelper, "Search", "Edit", "ErrorPartial", () => repository.SaveStaffingLog(request));
        }


        [HttpPost]
        public ActionResult AddProviders(int staffingLogId, IEnumerable<int> providers)
        {
            repository.AddProviders(staffingLogId, providers);
            return Content(string.Empty);
        }


        [HttpPost]
        public ActionResult RemoveProviders(int staffingLogId, IEnumerable<int> providers)
        {
            repository.RemoveProviders(staffingLogId, providers);
            return Content(string.Empty);
        }


        [HttpGet]
        public ActionResult LoadProviderContactLog(int caseId, ProviderTypeIDs type)
        {
            var data = service.LoadProviderContactLogVM(caseId, type);
            return new DateFormattedJsonResult
            {
                Data = data,
                DateFormat = "MM/dd/yyyy"
            };
        }


        [HttpGet]
        public ActionResult AddProviderContactLog(int caseId, ProviderTypeIDs type)
        {
            var model = service.CreateProviderContactLogVM(caseId, type);
            return PartialView("AddProviderContactLog", model);
        }


        [HttpPost]
        public ActionResult AddProviderContactLog(AddProviderContactLogVM model)
        {
            if (!ModelState.IsValid)
            {
                return PartialView("AddProviderContactLog", model);
            }
            service.SaveProviderContactLog(model);
            return new HttpStatusCodeResult(HttpStatusCode.Created);
        }


        [HttpGet]
        public ActionResult LoadParentContactLog(int caseId)
        {
            var data = service.LoadParentContactLogVM(caseId);
            return new DateFormattedJsonResult
            {
                Data = data,
                DateFormat = "MM/dd/yyyy"
            };
        }


        [HttpGet]
        public ActionResult AddParentContactLog(int caseId)
        {
            var model = service.CreateParentContactLogVM(caseId);
            return PartialView("AddParentContactLog", model);
        }


        [HttpPost]
        public ActionResult AddParentContactLog(AddParentContactLogVM model)
        {
            if (!ModelState.IsValid)
            {
                return PartialView("AddParentContactLog", model);
            }
            service.SaveParentContactLog(model);
            return new HttpStatusCodeResult(HttpStatusCode.Created);
        }


        [HttpGet]
        public JsonResult GetTimelineEntries(int caseId, bool includeNotes)
        {
            var items = TimelineService.GetTimelineEntries(caseId, includeNotes);
            return this.CamelCaseJson(items, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public ActionResult EmailProvidersModal(IEnumerable<int> ids)
        {
            var providers = service.GetProviders(ids);
            return PartialView("EmailProvidersModal", providers);
        }


        [HttpPost]
        public ActionResult LogEmail(IEnumerable<int> ids, string text)
        {
            service.LogEmailSent(ids, text);
            return Json(new { success = true });
        }


        /**********************
         *  CONTROLLER SETUP
         * *******************/
        private readonly StaffingRepository repository;
        private readonly StaffingService service;
        private readonly CaseTimelineService TimelineService;

        public StaffingController()
        {
            repository = new StaffingRepository();
            service = new StaffingService();
            TimelineService = new CaseTimelineService(AppService.Current.DataContextV2);
        }

    }

}