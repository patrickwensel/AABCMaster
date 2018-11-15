using AABC.Data.V2;
using AABC.Domain2.Providers;
using AABC.Domain2.Services;
using AABC.DomainServices.Authorizations;
using AABC.ProviderPortal.App.Hours;
using AABC.ProviderPortal.Models.Home;
using AABC.ProviderPortal.Repositories;
using Dymeng.Framework;
using Dymeng.Framework.Web.Mvc.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;


namespace AABC.ProviderPortal.Controllers
{
    public class HomeController : ContentBaseController
    {

        #region Controller Setup
        const int SSG_SERVICE_ID = (int)ServiceIDs.SocialSkillsGroup;
        const int BCBA_PROVIDER_TYPE_ID = (int)ProviderTypeIDs.BoardCertifiedBehavioralAnalyst;

        private readonly HomeRepository homeRepository;
        private readonly CoreContext _context;
        private readonly HoursService _service;

        private Domain.Providers.Provider _provider;
        private Domain.Providers.Provider Provider
        {
            get
            {
                if (_provider == null)
                {
                    _provider = Global.Default.GetUserProvider();
                }
                return _provider;
            }
        }

        private bool IsBCBA { get { return Provider.Type.ID == BCBA_PROVIDER_TYPE_ID; } }
        private DateTime CurrentHoursHistoryCutoff
        {
            get
            {
                // only show log and calendar history for current month and last month
                // get the furthest history date to retrieve hours for
                var currentDate = DateTime.Now;
                var firstOfCurrentMonth = new DateTime(currentDate.Year, currentDate.Month, 1);
                var firstOfPreviousMonth = firstOfCurrentMonth.AddMonths(-1);
                return firstOfPreviousMonth;
            }
        }
        public int ProviderID { get { return Provider.ID.Value; } }
        public DateTime ActivePeriod
        {
            get
            {
                var now = DateTime.Now;
                if (now.Day <= 10)
                {
                    var lastMonth = now.AddMonths(-1);
                    return new DateTime(lastMonth.Year, lastMonth.Month, 1);
                }
                else
                {
                    return new DateTime(now.Year, now.Month, 1);
                }
            }
        }
        public Provider ActiveProvider
        {
            get
            {
                return _context.Providers.Find(ProviderID);
            }
        }


        public HomeController()
        {
            _context = AppService.Current.Context;
            _service = new HoursService(_context);
            try
            {
                homeRepository = new HomeRepository(_context);
            }
            catch (NullReferenceException)
            {
                Exceptions.LogMessageToTelementry("NRE thrown on new Controllers.Home.ctor()=>new Repositories.HomeRepository()");
                Redirect("/Account/Logoff");
            }
            catch (Exception e)
            {
                Exceptions.Handle(e, Global.GetWebInfo());
                Redirect("/Account/Logoff");
            }

            try
            {
                _provider = Global.Default.GetUserProvider();
            }
            catch (NullReferenceException)
            {
                Exceptions.LogMessageToTelementry("NRE thrown on new Controllers.Home.ctor()=>Global.Default.GetUserProvider");
                Redirect("/Account/Logoff");
            }
            catch (Exception e)
            {
                Exceptions.Handle(e, Global.GetWebInfo());
                Redirect("/Account/Logoff");
            }
        }
        #endregion


        [HttpGet]
        public JsonResult GetAideList(int caseID)
        {
            var aides = _context.Cases.Find(caseID)
                .Providers
                .Where(x => (x.EndDate == null || x.EndDate >= DateTime.Now.AddMonths(-3)))
                .Select(x => x.Provider)
                .Where(x => x.IsAide)
                .ToList();
            var results = aides.Select(aide => new AideListItem
            {
                Name = aide.FirstName + " " + aide.LastName,
                Email = aide.Email,
                Phone = aide.Phone
            });
            return Json(results, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public ActionResult Index()
        {
            return RedirectToAction("Cases");
        }


        [HttpGet]
        public ActionResult Cases()
        {
            var providerID = Global.Default.User().ProviderID;
            var cases = homeRepository.GetCaseListByProvider(providerID.Value);
            var model = new MyCasesVM
            {
                IsBCBA = Global.Default.GetUserProvider().Type.Code == "BCBA" ? "true" : "false"
            };
            model.FillCases(cases);
            ViewBag.ServiceList = homeRepository.GetServiceList(Global.Default.GetUserProvider().Type.ID.Value);
            return GetView("Cases", model);
        }


        [HttpGet]
        public ActionResult CaseHoursEntry(int caseID)
        {
            ViewBag.IsBCBA = IsBCBA;
            ViewBag.ExtendedNotesFeatureEnabled = true;
            return PartialView("CaseHoursEntryCalendarContainer", Models.Home.HoursCalendar.SchedulerDataHelper.DataObject(caseID, Global.Default.GetUserProvider().ID.Value));
        }


        [HttpGet]
        public ActionResult GetTotalPendingHours(int caseID)
        {
            var hours = ActiveProvider.GetPendingHoursForCase(caseID);
            return Content(hours.ToString("#.##"));
        }


        [HttpGet]
        public ActionResult DownloadHoursRecord(int caseID)
        {
            var data = _service.GetHoursDownloadItems(ActiveProvider.ID, caseID, ActivePeriod.Month, ActivePeriod.Year);
            var settings = _service.GetHoursDownloadSettings(data, caseID, ActivePeriod);
            return DevExpress.Web.Mvc.GridViewExtension.ExportToXlsx(settings, data);
        }


        [HttpPost]
        public ActionResult CommitPendingHours(int caseID)
        {
            _service.CommitPendingHours(caseID, ProviderID);
            return Content("ok");
        }


        [HttpPost]
        public ActionResult CaseHoursDelete(int hoursID)
        {
            _service.DeleteHours(hoursID);
            return Content("ok");
        }


        [HttpGet]
        public ActionResult IsFinalized(int caseID, DateTime date)
        {
            // pre-refactor went to first of month
            // not sure if it didn't do the "ActivePeriod" style date for a reason
            // or as an oversight... for now, we'll do the same
            var firstDayOfMonth = new DateTime(date.Year, date.Month, 1, 0, 0, 0);
            if (_service.IsFinalized(caseID, ProviderID, firstDayOfMonth))
            {
                return Content("true");
            }
            else
            {
                return Content("false");
            }
        }


        [HttpGet]
        public ActionResult CanCreate(int caseID, DateTime date)
        {
            var isInactive = true;
            var c = _context.Cases.Find(caseID);
            var p = c.GetProvidersAtDate(date);
            if (p.Any(x => x.ProviderID == AppService.Current.CurrentProvider.ID))
            {
                isInactive = false;
            }
            var firstDayOfMonth = new DateTime(date.Year, date.Month, 1, 0, 0, 0);
            var isFinalized = _service.IsFinalized(caseID, ProviderID, firstDayOfMonth);
            if (isFinalized || isInactive)
            {
                return Content("false");
            }
            else
            {
                return Content("true");
            }
        }


        [HttpGet]
        public ActionResult ValidateFinalizeMonth(int caseID, int year, int month)
        {
            var firstDayOfMonth = new DateTime(year, month, 1);
            var count = _service.GetCountOfHours(caseID, ProviderID, firstDayOfMonth);
            if (count == 0)
            {
                return Content("No hours entered for this period, unable to finalize");
            }
            else
            {
                return Content("ok");
            }
        }


        [HttpGet]
        public ActionResult FinalizeMonthPopup(int caseID)
        {
            var model = homeRepository.GetFinalizeMonthPopupVM(caseID);
            var dupes = model.Items
                .GroupBy(x => x.MonthName)
                .Where(x => x.Count() > 1)
                .Select(x => x.Key);
            var toRemove = new List<FinalizedMonthItem>();

            foreach (var dupe in dupes)
            {
                foreach (var i in model.Items)
                {
                    if (i.MonthName == dupe)
                    {
                        if (i.IsFinalized == false)
                        {
                            if (toRemove.Where(x => x.MonthName == i.MonthName).FirstOrDefault() == null)
                            {
                                toRemove.Add(i);
                            }
                        }
                    }
                }
            }
            if (toRemove.Count > 0)
            {
                foreach (var r in toRemove)
                {
                    model.Items.Remove(r);
                }
            }
            return PartialView("FinalizeMonthPopup", model);
        }


        [HttpPost]
        public string FinalizeMonthSignature(int caseID, int year, int month)
        {
            var firstDayOfMonth = new DateTime(year, month, 1);
            var finalizationID = homeRepository.BeginFinalize(caseID, firstDayOfMonth, Provider.ID.Value);
            var reqUrl = Request.Url.OriginalString;
            var returnURL = reqUrl.Substring(0, reqUrl.IndexOf(Request.Url.AbsolutePath));
            returnURL = returnURL + "/Signing/Complete/" + finalizationID.ToString();
            var envelope = _service.FinalizeMonthSignature(caseID, firstDayOfMonth, returnURL);
            homeRepository.UpdateFinalizeWithEnvelopeID(finalizationID, envelope.EnvelopeID);
            return envelope.RedirectURL;
        }


        [Route("Signing/Complete/{finalizationID:int}")]
        public ActionResult FinalizationComplete(int finalizationID)
        {
            // get envelope ID from finalization
            var envelopeID = homeRepository.GetEnvelopeIDForFinalization(finalizationID);
            var dsClient = new Dymeng.DocuSign.DocuSignClient();
            var savePath = AppService.Current.Settings.ProviderSignedHoursPath + "\\" + finalizationID;
            var dsSettings = AppService.Current.Settings.DocuSignProviderFinalize;
            dsClient.AuthConfig = new Dymeng.DocuSign.AuthConfig()
            {
                Host = dsSettings.AuthHost,
                IntegratorKey = dsSettings.AuthIntegratorKey,
                OAuthBasePath = dsSettings.AuthOAuthBasePath,
                PrivateKeyPath = dsSettings.AuthPrivateKeyPath,
                UserID = dsSettings.AuthUserID
            };

            var signingSuccess = dsClient.GetDocumentAndStatus(envelopeID, savePath);
            if (signingSuccess)
            {
                homeRepository.CompleteFinalize(finalizationID);
            }

            var providerID = Global.Default.User().ProviderID;
            var cases = homeRepository.GetCaseListByProvider(providerID.Value);
            var model = new MyCasesVM
            {
                IsBCBA = Global.Default.GetUserProvider().Type.Code == "BCBA" ? "true" : "false",
                SigningSuccess = signingSuccess
            };
            model.FillCases(cases);
            ViewBag.ServiceList = homeRepository.GetServiceList(Global.Default.GetUserProvider().Type.ID.Value);
            return View("Cases", model);
        }


        [HttpPost]
        public string PostFinalization(int caseID, int year, int month)
        {
            var firstDayOfMonth = new DateTime(year, month, 1);
            var finalizationID = homeRepository.BeginFinalize(caseID, firstDayOfMonth, Provider.ID.Value);
            homeRepository.CompleteFinalize(finalizationID);
            return "ok";
        }


        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult CaseHoursEntrySchedulerCallback()
        {
            var caseID = int.Parse(Request.Params["caseID"]);
            return PartialView("CaseHoursEntryCalendar", Models.Home.HoursCalendar.SchedulerDataHelper.DataObject(caseID, Global.Default.GetUserProvider().ID.Value));
        }


        [HttpGet]
        public ActionResult CaseHoursLogs(int caseID, bool showAll)
        {
            var cutoff = CurrentHoursHistoryCutoff;
            var model = new CaseHoursDetailVM
            {
                ShowAllHours = showAll,
                IsSupervisor = homeRepository.IsProviderSupervisor(caseID, Provider.ID.Value),
                Items = homeRepository.GetCaseHoursDetails(caseID, Provider.ID.Value, cutoff)
            };
            return PartialView("CaseHoursLog", model);
        }


        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult CaseHoursLogCallback()
        {
            //var showAll = Request.Params["showAll"] == "true";
            var caseID = int.Parse(Request.Params["caseID"]);
            var provider = Global.Default.GetUserProvider();
            var cutoff = CurrentHoursHistoryCutoff;
            var model = new CaseHoursDetailVM
            {
                Items = homeRepository.GetCaseHoursDetails(caseID, provider.ID.Value, cutoff)
            };
            return PartialView("CaseHoursLogGrid", model.Items);
        }


        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult CaseGeneralHours(int caseID)
        {
            var matrixService = new AuthHoursMatrix(AppService.Current.Context.Database.Connection.ConnectionString, 0);
            var model = matrixService.GetMatrixItems(caseID, true);
            return PartialView("CaseGeneralHoursGrid", model);
        }


        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult CasesGridCallback()
        {
            var providerID = Global.Default.User().ProviderID;
            var cases = homeRepository.GetCaseListByProvider(providerID.Value);
            var model = new MyCasesVM();
            model.FillCases(cases);
            return PartialView("CasesGrid", model.Cases);
        }


    }

    public class Response
    {
        public bool Success { get; set; }
        public string Error { get; set; }
    }


    public class HoursEntryException : Exception
    {
        public HoursEntryException() : base() { }
        public HoursEntryException(string message) : base(message) { }
        public HoursEntryException(string message, Exception innerException) : base(message, innerException) { }
    }
}