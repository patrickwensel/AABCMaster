using AABC.Data.Models;
using AABC.Data.V2;
using AABC.Domain.Admin;
using AABC.Domain.General;
using AABC.Domain2.Providers;
using AABC.DomainServices.Utils;
using AABC.Web.App.Hours.Models;
using AABC.Web.App.Providers;
using AABC.Web.App.Providers.Models;
using AABC.Web.Models.Providers;
using AABC.Web.Models.Shared;
using AABC.Web.Repos;
using AABC.Web.Repositories;
using DevExpress.Web;
using DevExpress.Web.Mvc;
using Dymeng.Framework.Web.Mvc.Controllers;
using Dymeng.Framework.Web.Mvc.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace AABC.Web.Controllers
{

    public class ProvidersController : ContentBaseController
    {
        private readonly CoreContext Context;
        private readonly IProviderRepository ProviderRepository;
        private readonly ProviderService ProviderService;
        private readonly HoursRepo HoursRepository;

        public ProvidersController()
        {
            Context = AppService.Current.DataContextV2;
            ProviderRepository = new ProviderRepository();
            ProviderService = new ProviderService();
            HoursRepository = new HoursRepo();
        }


        #region Payroll

        [HttpGet]
        public ActionResult NewPayrollID()
        {
            return Content(ProviderService.GenerateNewPayrollID().ToString());
        }


        public ActionResult PayrollExportXlsx(DateTime targetDate, bool commit, string filter)
        {
            var filterValue = PayrollFilterListItem.GetEnumValue(filter);
            ProviderService.PayrollExportXlxs(targetDate, commit, filterValue, out GridViewSettings gridViewSettings, out IEnumerable<PayrollGridItemVM> data);
            return GridViewExtension.ExportToXlsx(gridViewSettings, data);
        }


        [HttpGet]
        public ActionResult Payroll()
        {
            var filtersList = PayrollFilterListItem.GetList();
            ViewBag.Push = new ViewModelBase(PushState, "/Providers/Payroll", "Payroll");
            ViewBag.AvailableDates = HoursRepository.GetScrubAvailableDates();
            ViewBag.FilterSelection = filtersList;
            ViewBag.SelectedFilter = filtersList.Where(x => x.Value == "none").Single();

            // if within 10 days of start of month, default to last month, otherwise use this month
            var targetDate = DateTime.Now;
            if (targetDate.Day <= 10)
            {
                targetDate = targetDate.AddMonths(-1);
            }
            ViewBag.SelectedDate = new AvailableDate()
            {
                Date = new DateTime(targetDate.Year, targetDate.Month, 1)
            };
            var model = new PayrollVM
            {
                Items = HoursRepository.GetPayablesByPeriod(new DateTime(targetDate.Year, targetDate.Month, 1))
            };
            return GetView("Payroll", model);
        }


        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult PayrollGridCallback(DateTime targetDate, string selectedFilter)
        {
            var filtersList = PayrollFilterListItem.GetList();
            ViewBag.FilterSelection = filtersList;
            ViewBag.SelectedFilter = filtersList.Where(x => x.Value == selectedFilter).Single();
            ViewBag.AvailableDates = HoursRepository.GetScrubAvailableDates();
            ViewBag.SelectedDate = new AvailableDate()
            {
                Date = targetDate
            };
            var filterEnumValue = PayrollFilterListItem.GetEnumValue(selectedFilter);
            var model = HoursRepository.GetPayablesByPeriod(new DateTime(targetDate.Year, targetDate.Month, 1), filterEnumValue);
            return PartialView("PayrollGrid", model);
        }
        #endregion



        [HttpGet]
        public ActionResult HoursReport(int caseID, int providerID, DateTime startPeriod, DateTime endPeriod)
        {
            if (!Global.Default.User().HasPermission(Permissions.ProviderHoursView))
            {
                return new HttpUnauthorizedResult();
            }
            using (var stream = ProviderService.GetHoursReports(caseID, providerID, startPeriod, endPeriod))
            {
                return File(stream.GetBuffer(), "application/pdf");
            }
        }


        [HttpGet]
        public ActionResult GetResume(int id)
        {
            var model = ProviderRepository.GetProvider(id);
            var filePath = AppService.Current.Settings.UploadDirectory + model.ResumeLocation;
            Response.AddHeader("Content-Disposition", "inline; filename=" + model.ResumeFileName);
            return File(System.IO.File.ReadAllBytes(filePath), ProviderService.GetMimeType(model.ResumeFileName));
        }



        #region InsuranceCredentials

        public ActionResult InsuranceCredentialGrid(int ProviderId)
        {
            IEnumerable<ProviderInsuranceCredentialDTO> list = new List<ProviderInsuranceCredentialDTO>();
            var provider = Context.Providers.SingleOrDefault(m => m.ID == ProviderId);
            if (provider != null)
            {
                list = provider.InsuranceCredentials.ToList().Select(ProviderInsuranceCredentialDTO.Map);
            }
            return PartialView("InsuranceCredentialGrid", list);
        }


        [HttpGet]
        public ActionResult InsuranceCredentialForm(int ProviderId, int InsuranceCredentialId)
        {
            // client requested the list be restricted to the following entries
            // this is a temporary hotfix while we add a card to the system to
            // handle this properly (refactor to latest context/db, add CanBeCredentialed flag to db/model...
            /* credential list and respective production IDs...
             * 
             * 79       Beacon      
             * 68       Horizon
             * 83, 84   CareFirst
             * 67       Empire BCBS HealthPlus
             * 71       United Healthcare
             * 92       Blue Cross Blue Shield
             */
            var idList = new List<int> { 79, 68, 83, 84, 67, 71, 92 };
            var context = new CoreEntityModel();
            var model = new ProviderInsuranceCredentialVM
            {
                InsuranceList = context.Insurances.Where(x => idList.Contains(x.ID)).ToList(),
                ProviderId = ProviderId
            };
            if (InsuranceCredentialId > 0)
            {
                var credentials = Context.ProviderInsuranceCredentials.SingleOrDefault(m => m.ID == InsuranceCredentialId);
                if (credentials != null)
                {
                    model.Id = credentials.ID;
                    model.ProviderId = credentials.ProviderID;
                    model.InsuranceId = credentials.InsuranceID;
                    model.StartDate = credentials.StartDate;
                    model.EndDate = credentials.EndDate;
                }
            }
            return PartialView("InsuranceCredentialForm", model);
        }


        [HttpPost]
        public ActionResult InsuranceCredentialForm(ProviderInsuranceCredentialVM model)
        {
            var c = new ProviderInsuranceCredential
            {
                InsuranceID = model.InsuranceId.GetValueOrDefault(0),
                ProviderID = model.ProviderId,
                StartDate = model.StartDate,
                EndDate = model.EndDate
            };
            Context.ProviderInsuranceCredentials.Add(c);
            //ProviderRepository.SaveInsuranceCredentials(c);
            return InsuranceCredentialGrid(model.ProviderId);
        }


        [HttpDelete]
        public ActionResult InsuranceCredentialDelete(int ProviderId, int InsuranceCredentialId)
        {
            var pic = Context.ProviderInsuranceCredentials.SingleOrDefault(m => m.ID == InsuranceCredentialId);
            if (pic != null)
            {
                Context.ProviderInsuranceCredentials.Remove(pic);
                Context.SaveChanges();
            }
            return InsuranceCredentialGrid(ProviderId);
        }
        #endregion



        [HttpPost]
        public ActionResult AddProviderRate(int providerID, DateTime effectiveDate, int rate)
        {
            ProviderRepository.AddProviderRate(providerID, effectiveDate, rate);
            return RedirectToAction("Edit", new { id = providerID });
        }


        [HttpPost]
        public ActionResult ValidateNameCollision(int? id, string firstName, string lastName)
        {
            // verify provider doesn't already exist
            if (ProviderService.IsNameCollision(id, firstName, lastName))
            {
                return Content("ERR: Provider name already exists, cannot duplicate");
            }
            else
            {
                return Content("ok");
            }
        }


        [HttpPost]
        public ActionResult Edit(ProviderVM model)
        {
            if (model.ID != 0)
            {
                model.Base = new ViewModelBase(PushState, "/Providers/Edit/" + model.ID, "Provider Editing", "/Providers/Search");
            }
            else
            {
                model.Base = new ViewModelBase(PushState, "/Providers/Search", "Provider Search");
            }

            var cases = model.ID != 0 ? ProviderRepository.GetProviderCaseDictionaryItems(model.ID) : new List<CaseVM>();
            model.ViewHelper.ActiveCasesDictionary = cases.Where(x => x.Active).ToDictionary(x => x.CaseID, x => x.PatientName);
            model.ViewHelper.InactiveCasesDictionary = cases.Where(x => !x.Active).ToDictionary(x => x.CaseID, x => x.PatientName);
            model.ViewHelper.ProviderTypesList = ProviderRepository.GetProviderTypesListItems();
            model.ViewHelper.StatesList = State.GetStates();

            if (model.ID != 0 && Global.Default.User().HasPermission(Permissions.CaseHoursView))
            {
                var selectedDate = GetDefaultTimeBillDate();
                ViewBag.SelectedDate = selectedDate;
                ViewBag.ProviderHours = ProviderRepository.GetProviderHours(model.ID, selectedDate.Date);
                ViewBag.AvailableDates = GetAvailableTimeBillDates();
            }
            return SaveAction(model, model.ViewHelper, "Search", "Edit", "ErrorPartial", () => ProviderRepository.SaveProvider(model));
        }


        [HttpPost]
        [Route("~/Provider/Types/Edit")]
        public ActionResult TypeEdit(ProviderTypeVM model)
        {
            return SaveAction(model, model.ViewHelper, "TypeSearch", "TypeEdit", "ErrorPartial", () => ProviderRepository.SaveProviderType(model));
        }


        [HttpPost]
        [Route("~/Providers/Types/Delete")]
        public ActionResult TypeDelete(int id)
        {
            ProviderRepository.DeleteProviderType(id);
            return RedirectToAction("TypeSearch");
        }


        [HttpGet]
        public ActionResult Index()
        {
            return RedirectToAction("Search");
        }


        [HttpGet]
        public ActionResult Search(ProviderFilter status = ProviderFilter.AllExceptInactive)
        {
            var model = new ProvidersListVM
            {
                DetailList = ProviderRepository.GetProviderListItems(status),
                Base = new ViewModelBase(PushState, "/Providers/Search", "Provider Search")
            };
            model.ListBase.CallbackFilterValue = status.ToString();
            model.ListBase.GridTitlePanelSettings.Title = $"Providers ({EnumHelper.GetTextAsString(status)})";
            model.ListBase.GridTitlePanelSettings.ShowAddButton = true;
            model.ListBase.GridTitlePanelSettings.AddNewAction = "Create";
            model.ListBase.GridTitlePanelSettings.AddNewController = "Providers";
            var filters = new List<ProviderFilter> { ProviderFilter.All, ProviderFilter.AllExceptInactive, ProviderFilter.Active, ProviderFilter.Inactive, ProviderFilter.Potential }
                            .Select(m => new GridTitlePanelFilterItem
                            {
                                RouteUrl = $"/Providers/Search/?status={m}",
                                Text = EnumHelper.GetTextAsString(m),
                                IsActive = status == m
                            }).ToList();
            model.ListBase.GridTitlePanelSettings.FilterItems = filters;
            var isFilterCallback = Request.Params["isFilterCallback"] == "true";
            return GetView(isFilterCallback ? "ListGrid" : "List", model);
        }


        public ActionResult GetSubTypes(int providerTypeID)
        {
            var model = new ProviderVM();
            model.ViewHelper.ProviderSubTypesList = ProviderRepository.GetSubTypes(providerTypeID);
            return PartialView("Edit_SubTypeComboBox", model);
        }


        [HttpGet]
        public ActionResult ProviderSubTypes()
        {
            var model = new ProviderSubTypesVM
            {
                Base = new ViewModelBase(PushState, $"/Providers/{nameof(ProviderSubTypes)}", "Provider Sub Types")
            };
            return GetView("ProviderSubTypes", model);
        }


        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult ProviderHoursGridCallback(int providerID, DateTime selectedPeriod)
        {
            ViewBag.AvailableDates = GetAvailableTimeBillDates();
            ViewBag.SelectedDate = new AvailableDate { Date = selectedPeriod };
            var hours = ProviderRepository.GetProviderHours(providerID, selectedPeriod);
            return PartialView("HoursGrid", hours);
        }


        [HttpGet]
        public ActionResult Edit(int id = 0)
        {
            var model = ProviderRepository.GetProvider(id);
            if (model == null)
            {
                return Create();
            }
            model.Base = new ViewModelBase(PushState, "/Providers/Edit/" + id, "Provider Editing", "/Providers/Search");
            model.ViewHelper.RateHistory = GetProviderRatesDictionary(id);
            model.ViewHelper.StatesList = State.GetStates();
            model.ViewHelper.SetServiceZips();
            model.ViewHelper.ProviderTypesList = ProviderRepository.GetProviderTypesListItems();

            if (model.Type.ID.HasValue)
            {
                model.ViewHelper.ProviderSubTypesList = ProviderRepository.GetSubTypes(model.Type.ID.Value);
            }

            var cases = ProviderRepository.GetProviderCaseDictionaryItems(id);
            model.ViewHelper.ActiveCasesDictionary = cases.Where(x => x.Active && x.Status != Domain2.Cases.CaseStatus.History).ToDictionary(x => x.CaseID, x => x.PatientName);
            model.ViewHelper.InactiveCasesDictionary = cases.Where(x => !x.Active).ToDictionary(x => x.CaseID, x => x.PatientName);
            if (Global.Default.User().HasPermission(Permissions.CaseHoursView))
            {
                var selectedDate = GetDefaultTimeBillDate();
                ViewBag.SelectedDate = selectedDate;
                ViewBag.ProviderHours = ProviderRepository.GetProviderHours(id, selectedDate.Date);
                ViewBag.AvailableDates = GetAvailableTimeBillDates();
            }
            return GetView("Edit", model);
        }


        [HttpGet]
        public ActionResult Create()
        {
            var model = new ProviderVM
            {
                Status = ProviderStatus.Active
            };
            model.ViewHelper.ProviderTypesList = ProviderRepository.GetProviderTypesListItems();
            model.ViewHelper.StatesList = State.GetStates();
            model.Base = new ViewModelBase(PushState, "/Providers/Create", "New Provider Entry");
            return GetView("Edit", model);
        }


        [HttpPost]
        public ActionResult Delete(int id)
        {
            if (!ProviderRepository.DeleteProvider(id))
            {
                return Content("false");
            }
            else
            {
                return Content("true");
            }
        }


        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult ListGridFilter()
        {
            return Search();
        }


        [HttpGet]
        public JsonResult GetDetails(int providerID)
        {
            var provider = Context.Providers.SingleOrDefault(m => m.ID == providerID);
            var data = new ProviderDetailsDTO
            {
                Type = provider.GetProviderTypeFullCode(),
                Name = provider.FirstName + " " + provider.LastName,
                Address = provider.Address1,
                Address2 = provider.Address2,
                ZipCode = provider.Zip,
                City = provider.City,
                State = provider.State,
                Phone = provider.Phone,
                Email = provider.Email,
                ServiceZipCodes = provider.ServiceZipCodes.Select(m => m.ZipCode).Distinct().OrderBy(m => m).ToList(),
                ServiceAreas = provider.ServiceZipCodes.Select(m => m.ZipInfo.County).Distinct().OrderBy(m => m).ToList(),
                InsuranceCredentials = provider.InsuranceCredentials.Select(m => ProviderInsuranceCredentialDTO.Map(m)).OrderByDescending(m => m.IsActive).OrderBy(m => m.InsuranceName)
            };
            return this.CamelCaseJson(data, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        [Route("~/Providers/Types/AssociatedCount")]
        public ActionResult TypeAssocCount(int id)
        {
            int i = ProviderRepository.GetProviderTypeAssociationCount(id);
            return Content(i.ToString());
        }


        public static readonly UploadControlValidationSettings ValidationSettings = new UploadControlValidationSettings
        {
            AllowedFileExtensions = new string[] { ".pdf", ".doc", ".docx", ".txt", ".rtf" },
            MaxFileSize = 20971520,
        };


        private IEnumerable<KeyValuePair<decimal, DateTime>> GetProviderRatesDictionary(int providerID)
        {
            var rates = Context.ProviderRates
                .Where(x => x.ProviderID == providerID)
                .OrderByDescending(x => x.EffectiveDate)
                .ToList()
                .Select(rate => new KeyValuePair<decimal, DateTime>(rate.Rate, rate.EffectiveDate));
            return rates;
        }


        private IEnumerable<AvailableDate> GetAvailableTimeBillDates()
        {
            return HoursRepository.GetScrubAvailableDates();
        }


        private AvailableDate GetDefaultTimeBillDate()
        {
            return new AvailableDate()
            {
                Date = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(-1)
            };
        }

    }
}