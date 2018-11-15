using AABC.DomainServices.Hours;
using AABC.Web.Helpers;
using AABC.Web.Reports;
using DevExpress.Web.Mvc;
using Dymeng.Framework;
using Dymeng.Framework.Web.Mvc.Controllers;
using Dymeng.Framework.Web.Mvc.Views;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AABC.Web.App.Hours
{
    [AuthorizePermissions(Domain.Admin.Permissions.ProviderHoursView)]
    public class HoursController : ContentBaseController
    {


        #region WATCH

        [Route("Hours/Watch/Grid/MissingBCBA")]
        public ActionResult WatchGridMissingBCBA(DateTime period)
        {
            var items = service.GetWatchGridMissingBCBAItems(period);
            if (items == null)
            {
                items = new List<Models.WatchCaseResultItem>();
            }
            items = items.Where(x => !x.Ignore).OrderBy(x => x.FirstName).ThenBy(x => x.LastName).ToList();
            return PartialView("WatchGridMissingBCBA", items);
        }


        [Route("Hours/Watch/Grid/MissingAide")]
        public ActionResult WatchGridMissingAide(DateTime period)
        {
            var items = service.GetWatchGridMissingAideItems(period)
                .Where(x => !x.Ignore)
                .OrderBy(x => x.FirstName)
                .ThenBy(x => x.LastName)
                .ToList();
            return PartialView("WatchGridMissingAide", items);
        }


        [Route("Hours/Watch/Grid/NoSupervision")]
        public ActionResult WatchGridMissingNoSupervision(DateTime period)
        {
            var items = service.GetWatchGridNoSupervisionItems(period)
                .Where(x => !x.Ignore)
                .OrderBy(x => x.FirstName)
                .ThenBy(x => x.LastName)
                .ToList();
            return PartialView("WatchGridNoSupervision", items);
        }


        [Route("Hours/Watch/Grid/NoHours")]
        public ActionResult WatchGridMissingNoHours(DateTime period)
        {
            var items = service.GetWatchGridNoHoursItems(period)
                .Where(x => !x.Ignore)
                .OrderBy(x => x.FirstName)
                .ThenBy(x => x.LastName)
                .ToList();
            return PartialView("WatchGridNoHours", items);
        }


        [Route("Hours/Watch/Grid/NoBilledHours")]
        public ActionResult WatchGridMissingNoBilledHours(DateTime period)
        {
            var items = service.GetWatchGridNoBilledHoursItems(period)
                .Where(x => !x.Ignore)
                .OrderBy(x => x.FirstName)
                .ThenBy(x => x.LastName)
                .ToList();
            return PartialView("WatchGridNoBilledHours", items);
        }


        public ActionResult Watch(DateTime? period)
        {
            // if within 10 days of start of month, default to last month, otherwise use this month
            var targetDate = DateTime.Now;
            if (DateTime.Now.Day <= 10)
            {
                targetDate = targetDate.AddMonths(-1);
            }
            var model = new Models.WatchVM
            {
                AvailableDates = repository.GetScrubAvailableDates(),
                SelectedDate = new Models.AvailableDate()
                {
                    Date = new DateTime(targetDate.Year, targetDate.Month, 1)
                }
            };
            ViewBag.Push = new ViewModelBase(PushState, "/Hours/Watch", "Hours Watch");
            ViewBag.AvailableDates = model.AvailableDates;
            ViewBag.SelectedDate = model.SelectedDate;
            return GetView("Watch", model);
        }


        public ActionResult WatchDetail(int caseID, int month, int year)
        {
            var caseService = new Case.CaseService();
            var caseProviders = caseService.GetCaseProviderListItems(caseID, true);
            foreach (var provider in caseProviders)
            {
                if (!string.IsNullOrEmpty(provider.Email))
                {
                    var mailtoEmail = HttpUtility.UrlEncode(provider.Email);
                    var displayEmail = HttpUtility.HtmlEncode(provider.Email);
                    provider.Email = "<a href='mailto:" + mailtoEmail + "'>" + displayEmail + "</a>";
                }
            }
            var caseMonthlyPeriod = caseService.GetCaseMonthlyPeriod(caseID, month, year);
            var model = new Models.WatchDetailPopupVM
            {
                Providers = caseProviders,
                CaseID = caseID,
                Month = month,
                Year = year
            };
            if (caseMonthlyPeriod != null)
            {
                model.Comments = caseMonthlyPeriod.WatchComment;
                model.Ignore = caseMonthlyPeriod.WatchIgnore;
            }
            return PartialView("WatchDetailPopup", model);
        }


        public void SaveWatchDetail(Models.WatchDetailPopupVM model)
        {
            var caseService = new Case.CaseService();
            caseService.SaveCaseMonthlyPeriod(model);
        }

        #endregion


        #region SCRUB

        public ActionResult Scrub()
        {
            // if within 10 days of start of month, default to last month, otherwise use this month
            var targetDate = DateTime.Now;
            if (DateTime.Now.Day <= 10)
            {
                targetDate = targetDate.AddMonths(-1);
            }
            var model = new Models.ScrubOverviewVM
            {
                AvailableDates = repository.GetScrubAvailableDates(),
                SelectedDate = new Models.AvailableDate()
                {
                    Date = new DateTime(targetDate.Year, targetDate.Month, 1)
                },
                Items = repository.GetScrubOverviewItems(
                new DateTime(targetDate.Year, targetDate.Month, 1),
                new DateTime(targetDate.Year, targetDate.Month, 1).AddMonths(1).AddDays(-1))
            };
            ViewBag.Push = new ViewModelBase(PushState, "/Hours/Scrub", "Hours Scrub");
            ViewBag.AvailableDates = model.AvailableDates;
            ViewBag.SelectedDate = model.SelectedDate;
            // default to first and last DOM of target date
            return GetView("ScrubOverview", model);
        }


        public ActionResult GetScrubListItemSummary(int caseID, DateTime selectedPeriod)
        {
            var model = new Models.ScrubOverviewItemSummaryVM(caseID)
            {
                PatientName = repository.GetPatientName(caseID)
            };
            repository.FillScrubSummaryProviderInfo(model, caseID, selectedPeriod);
            return PartialView("ScrubOverviewItemSummary", model);
        }


        public ActionResult DownloadUnfinalizedProviders(DateTime period)
        {
            var data = repository.GetUnfinalizedProviderExportItems(period);
            var s = new GridViewSettings
            {
                Name = "gvDownloadUnfinalizedProviders"
            };
            s.Settings.ShowFilterRow = false;
            s.SettingsBehavior.AllowSort = false;
            s.SettingsBehavior.AllowGroup = false;
            s.SettingsBehavior.AllowFocusedRow = false;
            s.SettingsBehavior.AllowSelectSingleRowOnly = true;
            s.SettingsExport.ExportSelectedRowsOnly = false;
            s.SettingsExport.FileName = "UnfinalizedProviders_" + DateTime.Now.ToString("yyyy-MM-dd") + ".xlsx";
            s.Columns.Add(col =>
            {
                col.Name = "colFirstName";
                col.FieldName = "FirstName";
                col.Caption = "First Name";
            });
            s.Columns.Add(col =>
            {
                col.Name = "colLastName";
                col.FieldName = "LastName";
                col.Caption = "Last Name";
            });
            s.Columns.Add(col =>
            {
                col.Name = "colEmail";
                col.FieldName = "Email";
                col.Caption = "Email";
            });
            s.Columns.Add(col =>
            {
                col.Name = "colHoursCount";
                col.FieldName = "HoursCount";
                col.Caption = "Hours Count";
            });
            s.Columns.Add(col =>
            {
                col.Name = "colHasFinalization";
                col.FieldName = "HasFinalization";
                col.Caption = "Has Finalization";
            });
            try
            {
                return GridViewExtension.ExportToXlsx(s, data);
            }
            catch (Exception e)
            {
                Exceptions.Handle(e, Global.GetWebInfo());
                throw e;
            }
        }


        public ActionResult MBHBillingPeriodPreview(DateTime period)
        {
            var data = repository.GetMHBExportItems(period, null);
            var s = new GridViewSettings
            {
                Name = "gvMBHBilling"
            };
            //s.KeyFieldName = "ID";
            s.SettingsBehavior.AllowSort = false;
            s.SettingsBehavior.AllowGroup = false;
            s.SettingsBehavior.AllowFocusedRow = false;
            s.SettingsBehavior.AllowSelectSingleRowOnly = true;
            s.Settings.ShowFilterRow = false;
            //s.CallbackRouteValues = new { Action = "PayrollGridCallback" };
            s.SettingsExport.ExportSelectedRowsOnly = false;
            s.SettingsExport.FileName = "MBH-Billing_" + DateTime.Now.ToString("yyyy-MM-dd") + ".xlsx";
            s.Columns.Add(col =>
            {
                col.Name = "colPatientFN";
                col.FieldName = "PatientFN";
            });
            s.Columns.Add("PatientLN");
            s.Columns.Add("ProviderFN");
            s.Columns.Add("ProviderLN");
            s.Columns.Add(col =>
            {
                col.Name = "colPatientID";
                col.FieldName = "PatientID";
                col.Caption = "Patient ID";
            });
            s.Columns.Add(col =>
            {
                col.Name = "colProviderID";
                col.FieldName = "ProviderID";
                col.Caption = "Provider ID";
            });
            s.Columns.Add("AuthorizedProviderID");

            s.Columns.Add(col =>
            {
                col.Name = "colSupervisingBCBAID";
                col.FieldName = "SupervisingBCBAID";
                col.Caption = "Supervising BCBA ID";
            });
            s.Columns.Add(col =>
            {
                col.Name = "colIsBCBATimesheet";
                col.FieldName = "IsBCBATimesheet";
                col.Caption = "Is BCBA Timesheet";
            });
            s.Columns.Add(col =>
            {
                col.Name = "colDateOfService";
                col.FieldName = "DateOfService";
                col.Caption = "Date Of Service";
            });
            s.Columns.Add(col =>
            {
                col.Name = "colStartTime";
                col.FieldName = "StartTime";
                col.Caption = "Start Time";
                col.PropertiesEdit.DisplayFormatString = "t";
            });
            s.Columns.Add(col =>
            {
                col.Name = "colEndTime";
                col.FieldName = "EndTime";
                col.Caption = "End Time";
                col.PropertiesEdit.DisplayFormatString = "t";
            });
            s.Columns.Add(col =>
            {
                col.Name = "colTotalTime";
                col.FieldName = "TotalTime";
                col.Caption = "Total Time";
            });
            s.Columns.Add(col =>
            {
                col.Name = "colServiceCode";
                col.FieldName = "ServiceCode";
                col.Caption = "Service Code";
            });
            s.Columns.Add(col =>
            {
                col.Name = "colPlaceOfService";
                col.FieldName = "PlaceOfService";
                col.Caption = "Place Of Service";
            });
            s.Columns.Add(col =>
            {
                col.Name = "colPlaceOfServiceID";
                col.FieldName = "PlaceOfServiceID";
                col.Caption = "Place Of Service ID";
            });
            try
            {
                return GridViewExtension.ExportToXlsx(s, data);
            }
            catch (Exception e)
            {
                Exceptions.Handle(e, Global.GetWebInfo());
                throw e;
            }
        }


        public ActionResult MBHBillingPeriodCommitAndExport(DateTime period)
        {
            var data = repository.GetMHBExportItems(period, null);
            var s = new GridViewSettings
            {
                Name = "gvMBHBilling"
            };
            //s.KeyFieldName = "ID";
            s.SettingsBehavior.AllowSort = false;
            s.SettingsBehavior.AllowGroup = false;
            s.SettingsBehavior.AllowFocusedRow = false;
            s.SettingsBehavior.AllowSelectSingleRowOnly = true;
            s.Settings.ShowFilterRow = false;
            //s.CallbackRouteValues = new { Action = "PayrollGridCallback" };
            s.SettingsExport.ExportSelectedRowsOnly = false;
            s.SettingsExport.FileName = "MBH-Billing_" + DateTime.Now.ToString("yyyy-MM-dd") + ".xlsx";
            s.Columns.Add(col =>
            {
                col.Name = "colPatientID";
                col.FieldName = "PatientID";
                col.Caption = "Patient ID";
            });
            s.Columns.Add(col =>
            {
                col.Name = "colProviderID";
                col.FieldName = "ProviderID";
                col.Caption = "Provider ID";
            });
            s.Columns.Add("AuthorizedProviderID");
            s.Columns.Add(col =>
            {
                col.Name = "colSupervisingBCBAID";
                col.FieldName = "SupervisingBCBAID";
                col.Caption = "Supervising BCBA ID";
            });
            s.Columns.Add(col =>
            {
                col.Name = "colIsBCBATimesheet";
                col.FieldName = "IsBCBATimesheet";
                col.Caption = "Is BCBA Timesheet";
            });
            s.Columns.Add(col =>
            {
                col.Name = "colDateOfService";
                col.FieldName = "DateOfService";
                col.Caption = "Date Of Service";
            });
            s.Columns.Add(col =>
            {
                col.Name = "colStartTime";
                col.FieldName = "StartTime";
                col.Caption = "Start Time";
                col.PropertiesEdit.DisplayFormatString = "t";
            });
            s.Columns.Add(col =>
            {
                col.Name = "colEndTime";
                col.FieldName = "EndTime";
                col.Caption = "End Time";
                col.PropertiesEdit.DisplayFormatString = "t";
            });
            s.Columns.Add(col =>
            {
                col.Name = "colTotalTime";
                col.FieldName = "TotalTime";
                col.Caption = "Total Time";
            });
            s.Columns.Add(col =>
            {
                col.Name = "colServiceCode";
                col.FieldName = "ServiceCode";
                col.Caption = "Service Code";
            });
            s.Columns.Add(col =>
            {
                col.Name = "colPlaceOfService";
                col.FieldName = "PlaceOfService";
                col.Caption = "Place Of Service";
            });
            s.Columns.Add(col =>
            {
                col.Name = "colPlaceOfServiceID";
                col.FieldName = "PlaceOfServiceID";
                col.Caption = "Place Of Service ID";
            });
            repository.CommitMHBPeriodExport(period);
            try
            {
                return GridViewExtension.ExportToXlsx(s, data);
            }
            catch (Exception e)
            {
                Exceptions.Handle(e, Global.GetWebInfo());
                throw e;
            }
        }


        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult ScrubGridCallback(DateTime targetDate)
        {
            var model = repository.GetScrubOverviewItems(
                new DateTime(targetDate.Year, targetDate.Month, 1),
                new DateTime(targetDate.Year, targetDate.Month, 1).AddMonths(1).AddDays(-1));
            ViewBag.AvailableDates = repository.GetScrubAvailableDates();
            ViewBag.SelectedDate = new Models.AvailableDate()
            {
                Date = targetDate
            };
            return PartialView("ScrubOverviewGrid", model);
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public void ScrubSelected(List<int> selectedHours)
        {
            service.ScrubSelected(selectedHours);
        }
        
        #endregion


        #region EDIT

        private ActionResult EditRecord(int id)
        {
            var model = service.GetHoursRecord(id);
            return PartialView("EditPopup", model);
        }


        public ActionResult SubmitEdit(Models.EditPopupSubmitVM submitData)
        {
            service.UpdateHoursSubmission(submitData);
            return Content("ok");
        }


        public ActionResult Edit(int? hoursID)
        {
            if (hoursID.HasValue)
            {
                return EditRecord(hoursID.Value);
            }
            var model = new Models.EditVM
            {
                ViewMode = Models.EditVM.ViewModes.FinalizedOnly,
                AvailableDates = service.GetAvailableDates()
            };
            model.SelectedDate = model.DefaultDate;
            model.Items = service.GetEditListItems(model.SelectedDate.Date);
            ViewBag.Push = new ViewModelBase(PushState, "/Hours/Edit", "Hours Edit");
            return GetView("Edit", model);
        }


        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult HoursEditGridCallback()
        {
            var period = DateTime.Parse(Request.Params["period"]);
            var viewMode = Request.Params["viewMode"];
            var model = new Models.EditVM
            {
                AvailableDates = service.GetAvailableDates(),
                SelectedDate = new Models.AvailableDate() { Date = period }
            };
            if (viewMode == nameof(Models.EditVM.ViewModes.AllHours))
            {
                model.ViewMode = Models.EditVM.ViewModes.AllHours;
            }
            else
            {
                model.ViewMode = Models.EditVM.ViewModes.FinalizedOnly;
            }
            if (model.ViewMode == Models.EditVM.ViewModes.FinalizedOnly)
            {
                model.Items = service.GetEditListItems(model.SelectedDate.Date, false);
            }
            else
            {
                model.Items = service.GetEditListItems(model.SelectedDate.Date, true);
            }
            return PartialView("EditGrid", model);
        }

        #endregion


        #region APPROVAL

        public ActionResult Approval()
        {
            var model = new Models.EditVM
            {
                ViewMode = Models.EditVM.ViewModes.FinalizedOnly,
                AvailableDates = service.GetAvailableDates()
            };
            model.SelectedDate = model.DefaultDate;
            model.Items = service.GetEditListItems(model.SelectedDate.Date);
            ViewBag.Push = new ViewModelBase(PushState, "/Hours/Approval", "Hours Approval");
            return GetView("Approval", model);
        }


        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult HoursApprovalGridCallback()
        {
            var period = DateTime.Parse(Request.Params["period"]);
            var viewMode = Request.Params["viewMode"];
            var model = new Models.EditVM
            {
                AvailableDates = service.GetAvailableDates(),
                SelectedDate = new Models.AvailableDate() { Date = period }
            };
            if (viewMode == nameof(Models.EditVM.ViewModes.AllHours))
            {
                model.ViewMode = Models.EditVM.ViewModes.AllHours;
            }
            else
            {
                model.ViewMode = Models.EditVM.ViewModes.FinalizedOnly;
            }
            if (model.ViewMode == Models.EditVM.ViewModes.FinalizedOnly)
            {
                model.Items = service.GetEditListItems(model.SelectedDate.Date, false);
            }
            else
            {
                model.Items = service.GetEditListItems(model.SelectedDate.Date, true);
            }
            return PartialView("ApprovalGrid", model);
        }


        [HttpGet]
        public ActionResult ParentApprovalReport(int approvalID)
        {
            var approval = AppService.Current.DataContextV2.ParentApprovals.Where(x => x.ID == approvalID).Single();
            var patient = approval.Hours.First().Case.Patient;
            var report = ReportService.GetParentApprovalReport(approval);
            using (var stream = new MemoryStream())
            {
                report.ExportToPdf(stream);
                Response.AddHeader("Content-Disposition", "inline; filename="
                    + patient.LastName + "_" + patient.FirstName + "_HoursApproval_"
                    + approval.Period.StartDate.Year + "-" + approval.Period.StartDate.Month + ".pdf");
                return File(stream.GetBuffer(), "application/pdf");
            }
        }

        #endregion


        #region REPORTED

        public ActionResult ResolvePopup(int id)
        {
            var model = service.GetResolveRecord(id);
            return PartialView("ResolvePopup", model);
        }


        public ActionResult ResolveSubmit(Models.ResolvePopupVM model)
        {
            service.UpdateResolveRecord(model);
            return Json("ok");
        }


        public ActionResult Reported()
        {
            var model = new Models.ReportedVM
            {
                Items = service.GetReportedListItems()
            };
            ViewBag.Push = new ViewModelBase(PushState, "/Hours/Reported", "Hours Reported");
            return GetView("Reported", model);
        }


        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult HoursReportedGridCallback()
        {
            var model = new Models.ReportedVM
            {
                Items = service.GetReportedListItems()
            };
            return PartialView("ReportedGrid", model);
        }
        
        #endregion


        #region FINALIZATIONS

        public ActionResult Finalizations()
        {
            var model = new Models.FinalizationsVM
            {
                AvailableDates = service.GetAvailableDates()
            };
            model.SelectedDate = model.DefaultDate;
            model.Items = service.GetFinalizationListItems(model.SelectedDate.Date);
            ViewBag.Push = new ViewModelBase(PushState, "/Hours/Finalizations", "Hours Finalizations");
            return GetView("Finalizations", model);
        }


        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult HoursFinalizationsGridCallback()
        {
            var period = DateTime.Parse(Request.Params["period"]);
            var model = new Models.FinalizationsVM
            {
                AvailableDates = service.GetAvailableDates(),
                SelectedDate = new Models.AvailableDate() { Date = period },
                Items = service.GetFinalizationListItems(period)
            };
            return PartialView("FinalizationsGrid", model);
        }


        [HttpGet]
        public ActionResult FinalizationDocs(int finalizationID)
        {
            using (var stream = service.GetFinalizationDocuments(finalizationID))
            {
                return File(stream.GetBuffer(), "application/pdf");
            }
        }

        #endregion


        #region VALIDATE

        public ActionResult Validate()
        {
            var model = new Models.ValidateVM
            {
                AvailableDates = repository.GetScrubAvailableDates()
            };
            model.SelectedDate = model.DefaultDate;
            model.Items = service.GetValidationItems(model.SelectedDate.Date, true);
            ViewBag.Push = new ViewModelBase(PushState, "/Hours/Validate", "Hours Validate");
            return GetView("Validate", model);
        }


        public ActionResult MaxPerDayPerPatient()
        {
            var model = new Models.ValidateVM
            {
                AvailableDates = repository.GetScrubAvailableDates()
            };
            model.SelectedDate = model.DefaultDate;
            ViewBag.Push = new ViewModelBase(PushState, "/Hours/MaxPerDayPerPatient", "Hours Validate");
            return GetView("MaxPerDayPerPatient", model);
        }


        public ActionResult MaxPerDayPerPatientList(DateTime period)
        {
            var model = service.GetPatientsExceedingMaxHoursPerDay(period);
            ViewData["period"] = period;
            return PartialView("MaxPerDayPerPatientList", model);

        }


        public ActionResult MaxPerDayPerPatientDays(int patientId, DateTime period)
        {
            var start = new DateTime(period.Year, period.Month, 1);
            var end = start.AddMonths(1).AddDays(-1);
            var model = svcHours.GetPatientExcessDays(patientId, start, end, 5);
            ViewData["patientId"] = patientId;
            ViewData["period"] = period;
            return PartialView("MaxPerDayPerPatientDays", model);
        }


        public ActionResult MaxPerDayPerPatientDayHours(int patientId, DateTime day)
        {
            var model = svcHours.GetPatientHours(patientId, day.Date, day.AddDays(1).Date);
            return PartialView("MaxPerDayPerPatientDayHours", model);
        }


        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult ScrubValidationGridCallback()
        {
            var period = DateTime.Parse(Request.Params["period"]);
            var model = new Models.ValidateVM
            {
                AvailableDates = repository.GetScrubAvailableDates(),
                SelectedDate = new Models.AvailableDate() { Date = period }
            };
            model.Items = service.GetValidationItems(model.SelectedDate.Date, false);
            return PartialView("ValidationResultsGrid", model);
        }


        public ActionResult RunValidation(DateTime period)
        {
            var model = new Models.ValidateVM
            {
                AvailableDates = repository.GetScrubAvailableDates(),
                SelectedDate = new Models.AvailableDate() { Date = period }
            };
            model.Items = service.GetValidationItems(model.SelectedDate.Date, true);
            return PartialView("ValidationResultsGrid", model);
        }

        #endregion


        #region PUBLIC API

        [HttpGet]
        [AllowAnonymous]
        public ActionResult ReconcileBreakdowns(string hoursIDList)
        {
            var resolver = new BatchAuthResolver(new Data.V2.CoreContext());
            resolver.ResolveAllAuths(hoursIDList.Split(';').Select(x => int.Parse(x)).ToList());
            return Content("ok");
        }

        #endregion


        #region ControllerSetup
        private readonly Repos.HoursRepo repository;
        private readonly HoursService service;
        private readonly HourService svcHours;

        public HoursController()
        {
            repository = new Repos.HoursRepo();
            service = new HoursService();
            svcHours = new HourService(AppService.Current.DataContextV2);
        }

        #endregion

    }
}