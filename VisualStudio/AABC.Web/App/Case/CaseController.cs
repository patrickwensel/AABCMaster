using AABC.Data.Mappings;
using AABC.DomainServices.Utils;
using AABC.Web.App.Case.Models;
using AABC.Web.Models.Cases;
using DevExpress.Web;
using Dymeng.Framework.Web.Mvc.Controllers;
using Dymeng.Framework.Web.Mvc.Views;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Mvc;

namespace AABC.Web.Controllers
{
    public partial class CaseController : ContentBaseController
    {

        private readonly Repositories.ICaseRepository repository;
        private readonly App.Case.CaseService _service;
        private readonly Repositories.IOfficeStaffRepository repoStaff;
        private readonly Data.Models.CoreEntityModel Context;


        public CaseController()
        {
            repository = new Repositories.CaseRepository();
            repoStaff = new Repositories.OfficeStaffRepository();
            _service = new App.Case.CaseService();
            Context = AppService.Current.DataContext;
        }


        #region CasesAndGeneral
        /*--------------------
            GETS
        ---------------------*/

        [HttpGet]
        public ActionResult Manage(int caseID, int? tabIndex)
        {
            ViewBag.CaseID = caseID;
            ViewBag.TabIndex = tabIndex;
            return GetView("ManagerContainer"); // view renders main via GetTab action
        }


        [HttpGet]
        public ActionResult ManageDirectRoute(int id, string view)
        {
            if (view == "Summary")
                return Manage(id, 0);
            if (view == "Providers")
                return Manage(id, 1);
            if (view == "NotesAndTracking")
                return Manage(id, 2);
            if (view == "Insurance")
                return Manage(id, 3);
            if (view == "PaymentPlans")
                return Manage(id, 4);
            if (view == "Payments")
                return Manage(id, 5);
            if (view == "TimeAndBilling")
                return Manage(id, 6);
            return Manage(id, 0);
        }


        [HttpGet]
        public ActionResult CaseDischarge(int caseID)
        {
            var model = new DischargePopupVM
            {
                CaseID = caseID,
                PatientName = repository.GetPatientNameByCaseID(caseID)
            };
            return PartialView("CaseDischargePopup", model);
        }


        /*--------------------
            CALLBACKS
        ---------------------*/
        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult GetTab()
        {
            // callback for casemanager tab control
            int tabID = int.Parse(Request.Params["tabIndex"]);
            int caseID = int.Parse(Request.Params["caseID"]);
            switch (tabID)
            {
                case 0:
                    return Summary(caseID);
                case 1:
                    return Providers(caseID);
                case 2:
                    return NotesTrack(caseID);
                case 3:
                    return Insurance(caseID);
                case 4:
                    return PaymentPlans(caseID, 0);
                case 5:
                    return Payments(caseID);
                case 6:
                    return TimeBill(caseID);
                default:
                    return Summary(caseID);
            }
        }


        /*--------------------
            POSTS
        ---------------------*/
        [HttpPost]
        public ActionResult Reactivate(int caseID)
        {
            var c = Context.Cases.Find(caseID);
            if (c != null)
            {
                c.CaseStatus = (int)Domain.Cases.CaseStatus.NotReady;
                Context.SaveChanges();
                new Repositories.PatientRepository().RecalculateCaseStatus(caseID);
            }
            return null;
        }


        [HttpPost]
        public ActionResult CaseDischarge(int caseID, string dischargeNotes)
        {
            try
            {
                repository.DischargeCase(caseID, dischargeNotes);
                return Content("ok");
            }
            catch
            {
                return Content("ERR: We're sorry, we've encountered an error processing this request.");
            }
        }
        #endregion


        #region AuthsAndGeneralHours
        /***********************
        * 
        * Authorizations & General Hours
        * 
        ************************/

        /*--------------------
            GETS
        ---------------------*/
        [HttpGet]
        public ActionResult CreateCaseAuthBase(int caseID)
        {
            var model = new AuthCreateVM
            {
                Detail = new Domain.Cases.CaseAuthorization()
            };
            model.Detail.AuthClass = new Domain.Cases.AuthorizationClass();
            model.ViewHelper.AuthClasses = repository.GetAuthClassList();
            model.ViewHelper.Authorizations = repository.GetAuthorizationsList(caseID);
            model.ViewHelper.CaseID = caseID;
            model.NewAuth = new Domain.Cases.Authorization();
            return PartialView("CaseAuthCreate", model);
        }


        [HttpGet]
        public ActionResult GeneralHoursEdit(int caseID, int authID)
        {
            var model = repository.GetGeneralHoursEdit(authID);
            model.ViewHelper.AuthClasses = repository.GetAuthClassList();
            model.ViewHelper.Authorizations = repository.GetAuthorizationsList(caseID);
            model.ViewHelper.CaseID = caseID;
            model.ViewHelper.FillListBlanks();
            model.ViewHelper.InitGeneralHours();
            model.NewAuth = new Domain.Cases.Authorization();
            return GetView("GeneralHoursEdit", model);
        }


        [HttpGet]
        public ActionResult GeneralHoursCreate(int caseID)
        {
            var model = new GeneralHoursEditVM
            {
                Detail = new Domain.Cases.CaseAuthorization()
            };
            model.ViewHelper.CaseID = caseID;
            model.ViewHelper.AuthClasses = repository.GetAuthClassList();
            model.ViewHelper.Authorizations = repository.GetAuthorizationsList(caseID);
            model.ViewHelper.FillListBlanks();
            model.ViewHelper.InitGeneralHours();
            model.NewAuth = new Domain.Cases.Authorization();
            return GetView("GeneralHoursEdit", model);
        }


        /*--------------------
            CALLBACKS
        ---------------------*/

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult GeneralHoursGridCallback(int caseID)
        {
            int authHoursGridShowFutureStartDays = int.TryParse(ConfigurationManager.AppSettings["AuthHoursGridShowFutureStartDays"], out authHoursGridShowFutureStartDays) ? authHoursGridShowFutureStartDays : 0;
            var matrixService = new DomainServices.Authorizations.AuthHoursMatrix(AppService.Current.DataContextV2.Database.Connection.ConnectionString, authHoursGridShowFutureStartDays);
            var table = matrixService.GetMatrixItems(caseID, false);
            return PartialView("ManageGeneralHoursGrid", table);
        }


        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult AllAuthsGridCallback(int caseID)
        {
            var data = _service.GetAllAuthsByCase(caseID);
            return PartialView("AuthsGrid", data);
        }


        //[AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        //public ActionResult GridCallback(int caseID, bool showHistory)
        //{
        //    if (showHistory)
        //    {
        //        return AllAuthsGridCallback(caseID);
        //    }
        //    return GeneralHoursGridCallback(caseID);
        //}

        /*--------------------
            POSTS
        ---------------------*/

        [HttpPost]
        public ActionResult CreateAuth(string authCode, string authDescription)
        {
            repository.CreateAuthorization(authCode, authDescription);
            return new EmptyResult();
        }


        [HttpPost]
        public ActionResult SubmitCaseAuthBase(AuthCreateVM model)
        {
            if (!model.ViewHelper.Validate())
            {
                throw new ArgumentException();
            }
            repository.SaveCaseAuthEntry(model);
            return Content("ok");
        }


        [HttpPost]
        public ActionResult GeneralHoursEdit(GeneralHoursEditVM model)
        {
            if (!model.ViewHelper.Validate())
            {
                throw new ArgumentException();
            }
            model.ViewHelper.BindModel();
            repository.SaveGeneralHoursEdit(model);
            return Content("ok");
        }


        /*--------------------
            DELETES
        ---------------------*/
        [HttpPost]
        public ActionResult DeleteAuth(int caseID, int authID)
        {
            repository.DeleteCaseAuthorization(caseID, authID);
            return RedirectToAction(nameof(Manage), new { caseID, tabIndex = 0 });
        }


        /*--------------------
            HELPERS
        ---------------------*/
        public ActionResult AuthValidCheck(int caseID)
        {
            List<Domain.Cases.CaseAuthorization> auths = repository.GetCaseAuthorizations(caseID);
            Domain.Cases.Case c = new Domain.Cases.Case
            {
                Authorizations = auths
            };
            if (c.HasAuthorization)
            {
                return Content("true");
            }
            else
            {
                return Content("false");
            }
        }
        #endregion


        #region Summary

        /***********************
        * 
        * Summary
        * 
        ************************/

        /*--------------------
            GETS
        ---------------------*/

        [HttpGet]
        public ActionResult Summary(int caseID)
        {
            var model = repository.GetCaseManagementSummary(caseID);
            model.HasAuthorizations = repository.GetCaseAuthorizations(caseID).Count > 0;
            model.Base = new ViewModelBase(PushState, "/Case/" + caseID + "/Manage/Summary", "Case Manager - Summary", "/Patients/Search");
            model.ViewHelper.StatusDescriptionList = repository.GetCaseStatusDescriptionList();
            model.ViewHelper.OfficeStaffList = repository.GetOfficeStaffList();
            model.ViewHelper.ServiceLocationList = repository.GetServiceLocationList();
            model.ViewHelper.FunctioningLevelList = repository.GetFunctioningLevelList();
            return GetView("ManageSummary", model);
        }

        /*--------------------
            POSTS
        ---------------------*/

        [HttpPost]
        public ActionResult SummaryEdit(SummaryVM model)
        {
            model.ViewHelper.BindModel();
            if (!model.ViewHelper.Validate())
            {
                model.ViewHelper.ReturnErrorMessage = "Please validate all fields before saving.";
                return RedirectToAction("Manage", new { caseID = model.ID.Value, tabIndex = 0 });
            }
            try
            {
                repository.SaveCaseManagementSummary(model);
                return RedirectToAction("Manage", new { caseID = model.ID.Value, tabIndex = 0 });
            }
            catch
            {
                model.ViewHelper.ReturnErrorMessage = "We encountered an unexpected problem while saving.  Please try again or contact your administrator if the problem persists.";
                return RedirectToAction("Manage", new { caseID = model.ID.Value, tabIndex = 0 });
            }
        }
        #endregion


        #region Providers
        /***********************
        * 
        * Providers
        * 
        ************************/

        /*--------------------
            GETS
        ---------------------*/

        [HttpGet]
        public ActionResult Providers(int caseID, bool showAll = false)
        {
            var caseOld = Context.Cases.Find(caseID);
            var patient = PatientMappings.Patient(caseOld.Patient);
            var model = new CaseProviderVM
            {
                ID = caseID,
                PatientName = patient.CommonName,
                PatientGender = patient.Gender.HasValue ? patient.Gender.Value.ToString().First().ToString() : string.Empty,
                Items = _service.GetCaseProviderListItems(caseID, showAll)
            };
            model.ViewHelper.Providers = _service.GetProvidersDropdown();
            model.Base = new ViewModelBase(PushState, "/Case/" + caseID + "/Manage/Providers", "Case Manager - Providers", "/Patients/Search");
            return GetView("ManageProviders", model);
        }


        /*--------------------
            CALLBACKS
        ---------------------*/

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult ProviderGridCallback(int caseID, bool showAll = false)
        {
            var model = _service.GetCaseProviderListItems(caseID, showAll);
            return PartialView("ManageProvidersGrid", model);
        }


        /*--------------------
            POSTS
        ---------------------*/



        [HttpPost]
        public ActionResult SetProviderStartDate(int caseProviderID, DateTime? newDate)
        {
            _service.CaseProviderSetStartDate(caseProviderID, newDate);
            return Content("ok");
        }

        [HttpPost]
        public ActionResult SetProviderEndDate(int caseProviderID, DateTime? newDate)
        {
            _service.CaseProviderSetEndDate(caseProviderID, newDate);
            return Content("ok");
        }

        [HttpPost]
        public ActionResult ToggleProviderCheckbox(int caseProviderID, string fieldName)
        {
            try
            {
                switch (fieldName)
                {
                    case "Active":
                        _service.CaseProviderToggleActive(caseProviderID);
                        break;
                    case "IsSupervisor":
                        _service.CaseProviderToggleIsSupervisor(caseProviderID);
                        break;
                    case "IsAssessor":
                        _service.CaseProviderToggleIsAssessor(caseProviderID);
                        break;
                    case "IsAuthorizedBCBA":
                        _service.CaseProviderToggleIsAuthorizedBCBA(caseProviderID);
                        break;
                    default:
                        throw new ArgumentException("field name not recognized");
                }
                return Content("ok");

            }
            catch (InvalidOperationException)
            {
                return Content("err");
            }
        }


        [HttpPost]
        public ActionResult CaseProviderAdd(int caseID, int providerID)
        {
            Domain.Cases.CaseProvider p = new Domain.Cases.CaseProvider
            {
                ID = providerID,
                Active = true
            };
            var providers = new List<Domain.Cases.CaseProvider>
            {
                p
            };
            repository.SaveCaseManagementProvidersEdit(providers, caseID);
            return RedirectToAction("Manage", new { caseID, tabIndex = 1 });
        }
        #endregion


        #region NotesAndTracking

        /***********************
        * 
        * Notes Tracking
        * 
        ************************/

        /*--------------------
            GETS
        ---------------------*/

        [HttpGet]
        public ActionResult NotesTrack(int caseID)
        {
            var c = Context.Cases.Find(caseID);
            var patient = PatientMappings.Patient(c.Patient);
            var model = new NotesVM
            {
                CaseID = c.ID,
                PatientID = c.PatientID,
                PatientName = patient.CommonName,
                PatientGender = patient.Gender.HasValue ? patient.Gender.Value.ToString().First().ToString() : string.Empty,
                Base = new ViewModelBase(PushState, "/Case/" + caseID + "/Manage/Notes", "Case Manager - Notes", "/Patients/Search")
            };
            return PartialView("ManageNotes", model);
        }
        #endregion


        #region TimeAndBilling


        /***********************
        * 
        * Time & Billing
        * 
        ************************/

        /*--------------------
            GETS
        ---------------------*/

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult TimeBillAutoPopulatePopup()
        {
            return PartialView("ManageTimeBillAutoPopulatePopup", new Models.Cases.TimeBillAutoPopulateVM());
        }

        [HttpGet]
        public ActionResult PatientHoursSignLineReport(int caseID, DateTime period)
        {
            if (!Global.Default.User().HasPermission(Domain.Admin.Permissions.ProviderHoursView))
            {
                return new HttpUnauthorizedResult();
            }
            using (var stream = _service.GetPatientHoursReportsForPeriod(caseID, period, App.Case.CaseService.PatientHoursReportConfigurations.SignLine))
            {
                return File(stream.GetBuffer(), "application/pdf");
            }
        }

        [HttpGet]
        public ActionResult PatientHoursSupervisingBCBAReport(int caseID, DateTime period)
        {
            if (!Global.Default.User().HasPermission(Domain.Admin.Permissions.ProviderHoursView))
            {
                return new HttpUnauthorizedResult();
            }
            using (var stream = _service.GetPatientHoursReportsForPeriod(caseID, period, App.Case.CaseService.PatientHoursReportConfigurations.SupervisingBCBA))
            {
                return File(stream.GetBuffer(), "application/pdf");
            }
        }


        [HttpGet]
        public ActionResult PatientHoursReport(int caseID, DateTime period)
        {
            if (!Global.Default.User().HasPermission(Domain.Admin.Permissions.ProviderHoursView))
            {
                return new HttpUnauthorizedResult();
            }
            using (var stream = _service.GetPatientHoursReportsForPeriod(caseID, period))
            {
                return File(stream.GetBuffer(), "application/pdf");
            }
        }

        [HttpGet]
        public ActionResult ParentHoursReport(int caseID, DateTime period)
        {
            if (!Global.Default.User().HasPermission(Domain.Admin.Permissions.ProviderHoursView))
            {
                return new HttpUnauthorizedResult();
            }
            using (var stream = _service.GetParentHoursReportsForPeriod(caseID, period))
            {
                return File(stream.GetBuffer(), "application/pdf");
            }
        }

        [HttpGet]
        public ActionResult AddHoursPopup(int caseID)
        {

            if (!HasTimeBillPermission)
            {
                throw new UnauthorizedAccessException();
            }


            var model = new Models.Cases.TimeBillAddHoursVM();

            var caseProviders = repository.GetCaseManagementProvidersList(caseID);

            model.cahpProviderList = new List<Domain.Providers.Provider>();
            foreach (var cp in caseProviders)
            {
                model.cahpProviderList.Add(new Domain.Providers.Provider() { ID = cp.ID, FirstName = cp.FirstName, LastName = cp.LastName });
            }

            model.cahpServiceList = repository.GetAllServicesExceptSSG();

            return PartialView("ManageTimeBillAddHoursPopup", model);

        }

        [HttpGet]
        public ActionResult TimeBill(int caseID)
        {
            if (!HasTimeBillPermission)
            {
                throw new UnauthorizedAccessException();
            }
            ViewBag.AvailableDates = GetAvailableTimeBillDates();
            ViewBag.SelectedDate = GetDefaultTimeBillDate();
            ViewBag.ServiceList = repository.GetAllServicesExceptSSG();
            ViewBag.CaseProviderList = repository.GetCaseManagementProvidersList(caseID);
            ViewBag.ServiceLocationList = repository.GetServiceLocationList();
            var model = _service.GetCaseTimeBillModel(caseID, GetDefaultTimeBillDate().Date);
            var services = repository.GetAllServicesExceptSSG();
            var auths = repository.GetCaseAuthorizationsForTimeBillGrid(caseID);
            ViewBag.ServiceList = services;
            ViewBag.CaseAuthList = auths;
            ViewBag.CaseID = caseID;
            ViewBag.HoursMatrix = model.HoursMatrix;
            var @case = Context.Cases.Find(caseID);
            var patient = PatientMappings.Patient(@case.Patient);
            model.PatientName = patient.CommonName;
            model.PatientGender = patient.Gender.HasValue ? patient.Gender.Value.ToString().First().ToString() : string.Empty;
            model.PatientID = @case.PatientID;
            model.Base = new ViewModelBase(PushState, "/Case/" + caseID + "/Manage/TimeAndBilling", "Case Manager - Time and Billing", "/Patients/Search");
            return GetView("ManageTimeBill", model);
        }


        [HttpGet]
        public ActionResult GetAuthResolutionDetails(int hoursId)
        {
            var data = _service.GetAuthResolutionDetails(hoursId);
            return this.CamelCaseJson(data, JsonRequestBehavior.AllowGet);
        }


        /*--------------------
            CALLBACKS
        ---------------------*/

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult TimeBillGridCallback()
        {

            if (!HasTimeBillPermission)
            {
                throw new UnauthorizedAccessException();
            }

            int caseID = Int32.Parse(Request.Params["caseID"]);
            DateTime periodStartDate = DateTime.Parse(Request.Params["selectedPeriod"]);
            var model = _service.GetCaseTimeBillModel(caseID, periodStartDate);

            var services = repository.GetAllServicesExceptSSG();
            var auths = repository.GetCaseAuthorizationsForTimeBillGrid(caseID);

            ViewBag.ServiceList = services;
            ViewBag.CaseAuthList = auths;
            ViewBag.CaseID = caseID;
            ViewBag.AvailableDates = GetAvailableTimeBillDates();
            ViewBag.SelectedDate = new App.Hours.Models.AvailableDate() { Date = periodStartDate };
            ViewBag.ServiceLocationList = repository.GetServiceLocationList();
            ViewBag.HoursMatrix = model.HoursMatrix;

            return PartialView("ManageTimeBillGrid", model.GridItems);
        }


        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult TimeBillGridBatchCallback(DevExpress.Web.Mvc.MVCxGridViewBatchUpdateValues<Models.Cases.TimeBillGridListItemVM, int> updateValues)
        {

            if (!HasTimeBillPermission)
            {
                throw new UnauthorizedAccessException();
            }

            int caseID = Int32.Parse(Request.Params["caseID"]);
            DateTime targetPeriodStartDate = DateTime.Parse(Request.Params["selectedPeriod"]);

            _service.BatchDeleteHours(updateValues.DeleteKeys);
            var issues = _service.TimeBillBatchUpdate(true, updateValues);

            if (issues.HasErrors)
            {
                var message = "We cannot save these hours:" + Environment.NewLine;
                message += issues.GetFriendlyErrors();
                throw new Exception(message);
            }

            var model = _service.GetCaseTimeBillModel(caseID, targetPeriodStartDate);

            var services = repository.GetAllServicesExceptSSG();
            var auths = repository.GetCaseAuthorizationsForTimeBillGrid(caseID);

            ViewBag.ServiceList = services;
            ViewBag.CaseAuthList = auths;
            ViewBag.CaseID = caseID;
            ViewBag.AvailableDates = GetAvailableTimeBillDates();
            ViewBag.SelectedDate = new App.Hours.Models.AvailableDate() { Date = targetPeriodStartDate };
            ViewBag.ServiceLocationList = repository.GetServiceLocationList();
            ViewBag.HoursMatrix = model.HoursMatrix;

            return PartialView("ManageTimeBillGrid", model.GridItems);
        }


        /*--------------------
            POSTS
        ---------------------*/



        [HttpPost]
        public ActionResult TimeBillDefinalizeProvider(int caseID, int providerID, DateTime firstDayOfPeriod)
        {

            try
            {

                repository.DefinalizeProvider(caseID, providerID, firstDayOfPeriod);

                return Content("ok");

            }
            catch (Exception e)
            {
                Dymeng.Framework.Exceptions.Handle(e, Global.GetWebInfo());
                throw e;
            }

        }

        [HttpPost]
        public ActionResult CaseHoursAutoPopPopulate(
            int caseID, int providerID, DateTime startDate, DateTime endDate,
            bool monday, bool tuesday, bool wednesday, bool thursday, bool friday, bool saturday, bool sunday,
            DateTime timeIn, DateTime timeOut, int serviceID, string notes)
        {


            try
            {

                Domain.Cases.CaseProvider caseProvider = new Domain.Cases.CaseProvider();
                var p = repository.GetCaseManagementProvidersList(caseID).Where(x => x.ID == providerID).First();
                caseProvider.ID = p.ID;
                caseProvider.Type = p.Type;

                var autoPopulateService = new Services.HoursAutopopulateService();
                List<Domain.Cases.CaseAuthorizationHours> items = autoPopulateService.GenerateHoursSet(
                    startDate, endDate, monday, tuesday, wednesday, thursday, friday, saturday, sunday);

                foreach (var item in items)
                {
                    item.CaseID = caseID;
                    item.Notes = notes;
                    item.Provider = caseProvider;
                    item.ProviderID = item.Provider.ID.Value;

                    item.Service = new Domain.Cases.Service() { ID = serviceID };
                    item.Status = Domain.Cases.AuthorizationHoursStatus.FinalizedByProvider;
                    item.TimeIn = new DateTime(item.Date.Year, item.Date.Month, item.Date.Day, timeIn.Hour, timeIn.Minute, 0);
                    item.TimeOut = new DateTime(item.Date.Year, item.Date.Month, item.Date.Day, timeOut.Hour, timeOut.Minute, 0);
                }

                int removedEntries = autoPopulateService.RemoveInvalidEntriesFromHoursSet(items);

                autoPopulateService.PopulateHours(items);

                return Content("ok, removed:" + removedEntries);

            }
            catch (Exception e)
            {

                Dymeng.Framework.Exceptions.Handle(e, Global.GetWebInfo());

                return Content("ERR:");

            }

        }

        [HttpPost]
        public ActionResult AddHoursSubmit(
            int caseID,
            int providerID,
            DateTime date,
            DateTime timeIn,
            DateTime timeOut,
            int serviceID,
            string notes,
            bool isAdjustment,
            bool? warningsAccepted)
        {

            if (!HasTimeBillPermission)
            {
                throw new UnauthorizedAccessException();
            }

            repository.AddCaseHoursViaAdmin(caseID, providerID, date, timeIn, timeOut, serviceID, notes, isAdjustment);

            return null;

        }

        [HttpPost]
        public ActionResult FinalizePeriodHours(int caseID, DateTime periodStartDate)
        {

            var repo = new Repos.HoursRepo();
            repo.FinalizeAllPerCaseAndPeriod(caseID, periodStartDate);

            return Content("OK");

        }



        /*--------------------
            HELPERS
        ---------------------*/

        private List<App.Hours.Models.AvailableDate> GetAvailableTimeBillDates()
        {
            return new Repos.HoursRepo().GetScrubAvailableDates();
        }


        private App.Hours.Models.AvailableDate GetDefaultTimeBillDate()
        {
            return new App.Hours.Models.AvailableDate()
            {
                Date = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(-1)
            };
        }


        bool HasTimeBillPermission
        {
            get
            {
                return Global.Default.User().HasPermission(Domain.Admin.Permissions.UserManagement);
            }
        }

        #endregion


        #region InsuranceDetails
        /***********************
        * 
        * Insurance Details
        * 
        ************************/

        [HttpGet]
        public ActionResult Insurance(int caseId)
        {
            var @case = repository.GetCaseManagementSummary(caseId);
            var model = new CaseInsuranceVM
            {
                CaseID = caseId,
                PatientName = @case.Patient.CommonName,
                PatientGender = @case.Patient.Gender.HasValue ? @case.Patient.Gender.ToString().First().ToString() : string.Empty,
                History = _service.GetInsuranceHistory(caseId),
                Base = new ViewModelBase(PushState, "/Case/" + caseId + "/Manage/Insurance", "Case Manager - Insurance", "/Patients/Search")
            };
            return PartialView("ManageInsurance", model);
        }


        [ChildActionOnly]
        public ActionResult InsuranceGrid(int caseId)
        {
            var model = _service.GetInsuranceHistory(caseId);
            return PartialView("ManageInsuranceGrid", model);
        }


        [HttpGet]
        public ActionResult GetInsurance(int caseInsuranceId)
        {
            var model = _service.GetInsurance(caseInsuranceId);
            return this.CamelCaseJson(model, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public ActionResult InsertInsurance(CaseInsuranceDTO caseInsuranceDTO)
        {
            object response;
            try
            {
                response = _service.InsertInsurance(caseInsuranceDTO);
            }
            catch (Exception e)
            {
                response = new { Error = e.Message };
            }
            return this.CamelCaseJson(response);
        }


        [HttpPost]
        public ActionResult SaveInsurance(CaseInsuranceDTO caseInsuranceDTO)
        {
            object response;
            try
            {
                response = _service.SaveInsurance(caseInsuranceDTO);
            }
            catch (Exception e)
            {
                response = new { Error = e.Message };
            }
            return this.CamelCaseJson(response);
        }


        [HttpPost]
        public ActionResult InsertCaseInsuranceMaxOutOfPocket(CaseInsuranceMaxOutOfPocketDTO insuranceMaxOutOfPocket)
        {
            object response;
            try
            {
                response = _service.InsertCaseInsuranceMaxOutOfPocket(insuranceMaxOutOfPocket);
            }
            catch (Exception e)
            {
                response = new { Error = e.Message };
            }
            return this.CamelCaseJson(response);
        }


        [HttpPost]
        public ActionResult SaveCaseInsuranceMaxOutOfPocket(CaseInsuranceMaxOutOfPocketDTO insuranceMaxOutOfPocket)
        {
            object response;
            try
            {
                response = _service.SaveCaseInsuranceMaxOutOfPocket(insuranceMaxOutOfPocket);
            }
            catch (Exception e)
            {
                response = new { Error = e.Message };
            }
            return this.CamelCaseJson(response);
        }


        [HttpPost]
        public ActionResult DeleteCaseInsuranceMaxOutOfPocket(int id)
        {
            object response;
            try
            {
                response = _service.DeleteCaseInsuranceMaxOutOfPocket(id);
            }
            catch (Exception e)
            {
                response = new { Error = e.Message };
            }
            return this.CamelCaseJson(response);
        }


        [HttpGet]
        public ActionResult GetMetadata()
        {
            var model = new
            {
                InsuranceCompanies = _service.GetInsurances().Select(m => new ListItem<int> { Value = m.ID, Text = m.Name }),
                InsuranceFundingTypes = EnumHelper.ToSelectListItem<Domain2.Cases.InsuranceFundingType>().Select(m => new ListItem<string> { Value = m.Text, Text = m.Text }),
                InsuranceBenefitTypes = EnumHelper.ToSelectListItem<Domain2.Cases.InsuranceBenefitType>().Select(m => new ListItem<string> { Value = m.Text, Text = m.Text })
            };
            return this.CamelCaseJson(model, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public ActionResult GetLocalCarriers(int id)
        {
            var model = _service.GetLocalCarriers(id).Select(m => new ListItem<int> { Value = m.ID, Text = m.Name });
            return this.CamelCaseJson(model, JsonRequestBehavior.AllowGet);
        }
        #endregion


        #region PaymentPlans



        /***********************
        * 
        * PaymentPlan Details
        * 
        ************************/

        /*--------------------
            GETS
        ---------------------*/
        public ActionResult PaymentPlans(int CaseId, int CasePaymentPlanId)
        {
            var model = _service.GetPaymentPlan(CaseId, CasePaymentPlanId);
            model.Base = new ViewModelBase(PushState, "/Case/" + CaseId + "/Manage/PaymentPlan", "Case Manager - PaymentPlan", "/Patients/Search");
            return PartialView("ManagePaymentPlans", model);
        }


        public ActionResult PaymentPlanForm(int CaseId, int CasePaymentPlanId)
        {
            var model = _service.GetPaymentPlan(CaseId, CasePaymentPlanId);
            model.Base = new ViewModelBase(PushState, "/Case/" + CaseId + "/Manage/PaymentPlan", "Case Manager - PaymentPlan", "/Patients/Search");
            return PartialView("ManagePaymentPlanForm", model);
        }


        public ActionResult PaymentPlanGrid(int CaseId)
        {
            var model = _service.GetPaymentPlans(CaseId);
            return PartialView("ManagePaymentPlanGrid", model);
        }


        public ActionResult BillingCorrespondenceList(int CaseId)
        {
            var model = _service.GetCaseBillingCorrespondences(CaseId);
            return PartialView("ManageBillingCorrespondenceList", model);
        }


        public ActionResult BillingCorrespondenceForm(int BillingCorrespondenceId, int CaseId)
        {
            var model = _service.GetCaseBillingCorrespondence(CaseId, BillingCorrespondenceId);
            model.CaseBillingCorrespondenceTypes = _service.GetCaseBillingCorrespondenceTypes();
            model.StaffList = repoStaff.GetOfficeStaffList();
            return PartialView("ManageBillingCorrespondenceForm", model);
        }


        [HttpGet]
        public ActionResult GetAttachment(int Id, string filename)
        {
            var model = _service.GetCaseBillingCorrespondence(0, Id);
            var filePath = AppService.Current.Settings.UploadDirectory + "\\case\\" + model.BillingCorrespondenceCaseId + "\\correspondence\\" + model.Id + "\\" + model.AttachmentFilename;
            Response.AddHeader("Content-Disposition", "inline; filename=" + model.AttachmentFilename);
            return File(System.IO.File.ReadAllBytes(filePath), GetMimeType(model.AttachmentFilename));
        }


        public ActionResult InsurancePaymentGrid(int CaseId)
        {
            var model = _service.GetCaseInsurancePayments(CaseId);
            return PartialView("ManageInsurancePaymentGrid", model);
        }


        public ActionResult InsurancePaymentForm(int InsurancePaymentId, int CaseId)
        {
            //var model = _service.GetCaseInsurancePayment(InsurancePaymentId);
            //model.CaseInsuranceList = _service.GetInsurances(CaseId);
            //if (model.CaseInsuranceList.Count > 0)
            //{
            //    model.PaymentCaseInsuranceId = model.CaseInsuranceList[0].ID;
            //}
            //return PartialView("ManageInsurancePaymentForm", model);
            //todo: make sure this is not used
            throw new NotImplementedException();
        }


        /*--------------------
            POSTS
        ---------------------*/
        [HttpPost]
        public ActionResult SavePaymentPlan(CasePaymentPlanVM paymentPlan)
        {
            _service.SavePaymentPlan(paymentPlan);
            return PaymentPlanForm(paymentPlan.PaymentPlanCaseId, paymentPlan.Id);
        }


        [HttpPost]
        public ActionResult BillingCorrespondenceSave(CaseBillingCorrespondenceVM m)
        {
            _service.SaveCaseBillingCorrespondence(m);
            return ManageDirectRoute(m.BillingCorrespondenceCaseId, "PaymentPlans");
        }


        [HttpPost]
        public ActionResult InsurancePaymentSave(CaseInsurancePaymentVM m)
        {
            _service.SaveCaseInsurancePaymentVM(m);
            return InsurancePaymentForm(m.PaymentCaseInsuranceId, m.Id);
        }



        /*--------------------
            DELETES
        ---------------------*/
        [HttpDelete]
        public ActionResult InsurancePayment(int Id)
        {
            //var model = _service.GetCaseInsuranceMaxOutOfPocket(Id);
            //_service.DeleteCaseInsurancePaymentVM(model.Id);
            //return InsurancePaymentGrid(model.CaseInsuranceId);
            //todo: make sure this is not used
            throw new NotImplementedException();
        }



        /*--------------------
            HELPERS
        ---------------------*/
        public static readonly UploadControlValidationSettings ValidationSettings = new UploadControlValidationSettings
        {
            AllowedFileExtensions = new string[] { ".pdf", ".doc", ".docx", ".txt", ".rtf" },
            MaxFileSize = 20971520,
        };


        private string GetMimeType(string fileName)
        {
            string extension = fileName.Substring(fileName.LastIndexOf('.')).ToLower();

            switch (extension)
            {
                case ".pdf":
                    return "application/pdf";

                default:
                    return "application/octet";
            }
        }
        #endregion


        #region Payments


        /***********************
        * 
        * Payments
        * 
        ************************/

        /*--------------------
            GETS
        ---------------------*/

        [HttpGet]
        public ActionResult Payments(int caseID)
        {



            var model = new App.Case.Models.CasePaymentVM
            {
                CaseID = caseID,
                Base = new ViewModelBase(
                PushState,
                "/Case/" + caseID + "/Manage/Payments",
                "Case Manager - Payments",
                "/Patients/Search")
            };
            return GetView("ManagePayments", model);
        }

        #endregion


    }

    public class Response
    {
        public bool Success { get; set; }
        public string Error { get; set; }
    }
}
