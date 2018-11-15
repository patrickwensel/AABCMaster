using AABC.DomainServices.Patients;
using AABC.Web.App.Hours.Models;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AABC.Web.App.Hours
{
    public class HoursService
    {

        const string VALIDATION_RESULT_CACHE_KEY_BASE = "HoursValidate_Results";

        Data.V2.CoreContext _context;

        public HoursService() {
            _context = AppService.Current.DataContextV2;
        }



        internal System.IO.MemoryStream GetFinalizationDocuments(int finalizationID) {
            
            var path = AppService.Current.Settings.ProviderPortalDocuSignFinalizationsRoot;
            path = System.IO.Path.Combine(path, finalizationID.ToString());

            var tempDir = AppService.Current.Settings.TempDirectory;
            var outputFilename = "Finalization_Documents_" + finalizationID.ToString() + ".pdf";
            var outputFullPath = System.IO.Path.Combine(tempDir, outputFilename);
            var files = System.IO.Directory.GetFiles(path);

            if (System.IO.File.Exists(outputFullPath)) {
                System.IO.File.Delete(outputFullPath);
            }

            MergePDFs(outputFullPath, files);

            using (var stream = new System.IO.MemoryStream()) {
                byte[] docBytes = System.IO.File.ReadAllBytes(outputFullPath);
                stream.Write(docBytes, 0, docBytes.Length);
                return stream;
            }
            
        }

        private static void MergePDFs(string targetPath, params string[] pdfs) {
            using (PdfDocument targetDoc = new PdfDocument()) {
                foreach (string pdf in pdfs) {
                    using (PdfDocument pdfDoc = PdfReader.Open(pdf, PdfDocumentOpenMode.Import)) {
                        for (int i = 0; i < pdfDoc.PageCount; i++) {
                            targetDoc.AddPage(pdfDoc.Pages[i]);
                        }
                    }
                }
                targetDoc.Save(targetPath);
            }
        }


        internal List<FinalizationListItem> GetFinalizationListItems(DateTime date) {

            var firstOfMonth = new DateTime(date.Year, date.Month, 1);
            
            var finalizations = _context.HoursFinalizations.Where(x => x.Period.FirstDayOfMonth == firstOfMonth).ToList();

            var items = new List<FinalizationListItem>();
            foreach (var finalization in finalizations) {
                items.Add(FinalizationListItem.MapFromDomain(finalization));
            }

            return items;
        }




        private string getValidationItemsCacheKey(DateTime period) {
            return VALIDATION_RESULT_CACHE_KEY_BASE + "_" + period.ToString("YYYYMMDD");
        }

        private List<ValidationItemVM> getValidationItemsFromCache(DateTime period) {
            var key = getValidationItemsCacheKey(period);
            return UserCache.GetItem(key) as List<ValidationItemVM>;
        }

        private void setValidateItemsCache(List<ValidationItemVM> items, DateTime period) {
            var key = getValidationItemsCacheKey(period);
            UserCache.AddItem(key, items);
        }

        public List<ValidationItemVM> GetValidationItems(DateTime period, bool fullRefresh) {

            if (!fullRefresh) {
                var cachedItems = getValidationItemsFromCache(period);
                if (cachedItems != null) {
                    return cachedItems;
                }
            }

            var items = new List<ValidationItemVM>();
            var caseService = new Data.Services.CaseService();
            var validator = new DomainServices.Hours.CrossHoursValidation(caseService, period);

            validator.Validate();

            int i = 0;

            foreach (var err in validator.Errors) {

                var item = new App.Hours.Models.ValidationItemVM();

                var coreContext = new Data.Models.CoreEntityModel();

                var sourcePatient = coreContext.Cases.Find(err.MatchedOn.CaseID.Value).Patient;
                var sourceProvider = coreContext.Providers.Find(err.MatchedOn.ProviderID);

                item.GridID = i;
                i++;
                item.SourceCaseID = err.MatchedOn.CaseID.Value;
                item.SourceDate = err.MatchedOn.Date;
                item.SourceHoursID = err.MatchedOn.ID.Value;
                item.SourcePatientName = sourcePatient.PatientFirstName + " " + sourcePatient.PatientLastName;

                item.SourceProviderID = err.MatchedOn.ProviderID;
                item.SourceProviderName = sourceProvider.ProviderFirstName + " " + sourceProvider.ProviderLastName;
                item.SourceServiceCode = err.MatchedOn.ServiceCode;
                item.SourceTimeIn = err.MatchedOn.TimeIn;
                item.SourceTimeOut = err.MatchedOn.TimeOut;

                item.TypeValue = err.ErrorType;

                if (err.MatchedTo != null && err.MatchedTo.Count > 0) {

                    var m = err.MatchedTo[0];

                    var patient = coreContext.Cases.Find(m.CaseID.Value).Patient;
                    var provider = coreContext.Providers.Find(m.ProviderID);

                    item.PartnerCaseID = m.CaseID.Value;
                    item.PartnerDate = m.Date;
                    item.PartnerHoursID = m.ID.Value;
                    item.PartnerPatientName = patient.PatientFirstName + " " + patient.PatientLastName;
                    item.PartnerProviderID = m.ProviderID;
                    item.PartnerProviderName = provider.ProviderFirstName + " " + provider.ProviderLastName;
                    item.PartnerServiceCode = m.ServiceCode;
                    item.PartnerTimeIn = m.TimeIn;
                    item.PartnerTimeOut = m.TimeOut;
                }

                items.Add(item);
            }

            setValidateItemsCache(items, period);

            return items;


        }

        public List<ValidationItemVM> GetValidationItems(DateTime period) {
            return GetValidationItems(period, true);
        }

        public List<PatientEx> GetPatientsExceedingMaxHoursPerDay(DateTime period)
        {
            PatientService svcPatient = new PatientService(AppService.Current.DataContextV2);
            return svcPatient.GetPatientsExceedingMaxHoursPerDay(period);
        }

        internal List<WatchCaseResultItem> GetWatchGridMissingAideItems(DateTime period) {

            DateTime startDate = new DateTime(period.Year, period.Month, 1);
            DateTime endDate = startDate.AddMonths(1).AddDays(-1);

            var context = AppService.Current.DataContextV2;
            var data = context.GetCasesWithNoAideHours(startDate, endDate);

            var items = new List<WatchCaseResultItem>();

            foreach (var d in data) {
                items.Add(new WatchCaseResultItem()
                {
                    CaseID = d.CaseID,
                    FirstName = d.PatientFirstName,
                    LastName = d.PatientLastName,
                    ProviderFirstName = d.ProviderFirstName,
                    ProviderLastName = d.ProviderLastName,
                    Comments = String.IsNullOrEmpty(d.WatchComment) ? "" : "...",
                    Ignore = d.WatchIgnore.HasValue ? d.WatchIgnore.Value : false
                });
            }

            return items;
        }

        internal List<WatchCaseResultItem> GetWatchGridMissingBCBAItems(DateTime period) {

            DateTime startDate = new DateTime(period.Year, period.Month, 1);
            DateTime endDate = startDate.AddMonths(1).AddDays(-1);

            var context = AppService.Current.DataContextV2;
            var data = context.GetCasesWithNoBCBAHours(startDate, endDate);

            var items = new List<WatchCaseResultItem>();

            foreach (var d in data) {
                items.Add(new WatchCaseResultItem()
                {
                    CaseID = d.CaseID,
                    FirstName = d.PatientFirstName,
                    LastName = d.PatientLastName,
                    ProviderFirstName = d.ProviderFirstName,
                    ProviderLastName = d.ProviderLastName,
                    Comments = String.IsNullOrEmpty(d.WatchComment) ? "" : "...",
                    Ignore = d.WatchIgnore.HasValue ? d.WatchIgnore.Value : false
                });
            }

            return items;
        }

        internal List<WatchCaseResultItem> GetWatchGridNoSupervisionItems(DateTime period) {

            DateTime startDate = new DateTime(period.Year, period.Month, 1);
            DateTime endDate = startDate.AddMonths(1).AddDays(-1);

            var context = AppService.Current.DataContextV2;
            var data = context.GetCasesWithHoursButNoSupervision(startDate, endDate);

            var items = new List<WatchCaseResultItem>();

            foreach (var d in data) {
                items.Add(new WatchCaseResultItem()
                {
                    CaseID = d.CaseID,
                    FirstName = d.PatientFirstName,
                    LastName = d.PatientLastName,
                    ProviderFirstName = d.ProviderFirstName,
                    ProviderLastName = d.ProviderLastName,
                    Comments = String.IsNullOrEmpty(d.WatchComment) ? "" : "...",
                    Ignore = d.WatchIgnore.HasValue ? d.WatchIgnore.Value : false
                });
            }

            return items;
        }

        internal List<WatchCaseResultItem> GetWatchGridNoHoursItems(DateTime period) {

            DateTime startDate = new DateTime(period.Year, period.Month, 1);
            DateTime endDate = startDate.AddMonths(1).AddDays(-1);

            var context = AppService.Current.DataContextV2;
            var data = context.GetCasesWithAuthButNoHours(startDate, endDate);

            var items = new List<WatchCaseResultItem>();

            foreach (var d in data) {
                items.Add(new WatchCaseResultItem()
                {
                    CaseID = d.CaseID,
                    FirstName = d.PatientFirstName,
                    LastName = d.PatientLastName,
                    Comments = String.IsNullOrEmpty(d.WatchComment) ? "" : "...",
                    Ignore = d.WatchIgnore.HasValue ? d.WatchIgnore.Value : false
                });
            }

            return items;
        }
        
        internal List<WatchCaseResultItem> GetWatchGridNoBilledHoursItems(DateTime period)
        {
            DateTime startDate = new DateTime(period.Year, period.Month, 1);
            DateTime endDate = startDate.AddMonths(1).AddDays(-1);

            var context = AppService.Current.DataContextV2;
            var data = context.GetCasesWithoutHoursBilled(startDate, endDate);

            return data.Select(d => new WatchCaseResultItem
                {
                    CaseID = d.CaseID,
                    FirstName = d.PatientFirstName,
                    LastName = d.PatientLastName,
                    ProviderLastName = d.ProviderLastName,
                    ProviderFirstName = d.ProviderFirstName,
                    Comments = String.IsNullOrEmpty(d.WatchComment) ? "" : "...",
                    Ignore = d.WatchIgnore ?? false
                })
                .ToList();
        }



        public List<App.Hours.Models.AvailableDate> GetAvailableDates() {
            DateTime now = DateTime.Now;
            DateTime last = new DateTime(now.Year, now.Month, 1).AddMonths(1);
            DateTime first = last.AddYears(-1);
            DateTime current = last;

            var items = new List<App.Hours.Models.AvailableDate>();
            while (current > first) {
                items.Add(new App.Hours.Models.AvailableDate() { Date = new DateTime(current.Year, current.Month, 1) });
                current = current.AddMonths(-1);
            }

            return items;
        }

        

        internal void UpdateHoursSubmission(EditPopupSubmitVM submitData) {
            
            var context = new Data.Models.CoreEntityModel();

            var item = context.CaseAuthHours.Find(submitData.id);

            item.HoursBillable = decimal.Parse(submitData.billableHours);
            item.HoursBillingRef = submitData.billingRef;
            item.HoursDate = submitData.date;
            item.HoursHasCatalystData = submitData.hasData;
            item.HoursNotes = submitData.notes;
            item.HoursPayable = decimal.Parse(submitData.payableHours);
            item.HoursServiceID = submitData.serviceID;
            item.HoursSSGParentID = submitData.ssgParentID;
            item.HoursStatus = (int)submitData.ConvertedStatus;
            item.HoursTimeIn = submitData.timeIn.TimeOfDay;
            item.HoursTimeOut = submitData.timeOut.TimeOfDay;
            item.HoursTotal = decimal.Parse(submitData.totalHours);
            item.ServiceLocationID = submitData.serviceLocationID;

            if (submitData.extendedNotes != null)
            {
                foreach (ExtendedNote n in submitData.extendedNotes)
                {
                    
                    if (n.id == 0 && !String.IsNullOrEmpty(n.value))
                    {
                        var dbNote = new Data.Models.CaseAuthHoursNote();
                        dbNote.HoursID = submitData.id;
                        dbNote.NotesTemplateID = n.templateId;
                        dbNote.NotesAnswer = n.value;

                        context.CaseAuthHoursNotes.Add(dbNote);
                    }
                    else if (n.id != 0 && !String.IsNullOrEmpty(n.value))
                    {
                        var dbNote = context.CaseAuthHoursNotes.Find(n.id);
                        dbNote.NotesAnswer = n.value;

                    }
                    else if (n.id != 0 && String.IsNullOrEmpty(n.value))
                    {
                        var dbNote = context.CaseAuthHoursNotes.Find(n.id);
                        context.CaseAuthHoursNotes.Remove(dbNote);
                    }
                    // else if (n.id == 0 && String.IsNullOrEmpty(n.value)) //Do nothing.
                }
            }


            context.SaveChanges();

            // now that the hours are updated, re-calc the auth breakdown
            var c2 = AppService.Current.DataContextV2;
            var h = c2.Hours.Find(submitData.id);
            var authResolver = new DomainServices.Hours.AuthResolver(h);
            authResolver.UpdateAuths(c2);

        }

        

        internal List<EditPopupVM.ServiceListItem> GetServicesList() {

            var context = new Data.Models.CoreEntityModel();
            var dataItems = context.Services.ToList();
            var items = new List<EditPopupVM.ServiceListItem>();

            foreach (var d in dataItems) {
                items.Add(new EditPopupVM.ServiceListItem()
                {
                    ID = d.ID,
                    Code = d.ServiceCode,
                    Name = d.ServiceName
                });
            }

            return items;
        }

        internal List<EditPopupVM.ServiceLocationListItem> GetServiceLocationsList() {

            var context = new Data.Models.CoreEntityModel();
            var dataItems = context.ServiceLocations.ToList();
            var items = new List<EditPopupVM.ServiceLocationListItem>();

            foreach (var d in dataItems) {
                items.Add(new EditPopupVM.ServiceLocationListItem()
                {
                    ID = d.ID,
                    Name = d.LocationName
                });
            }

            return items;
        }

        internal List<EditPopupVM.AuthListItem> GetAuthList(int caseID) {

            var context = new Data.Models.CoreEntityModel();
            var dataItems = context.CaseAuthCodes.Where(x => x.CaseID == caseID).ToList();
            var items = new List<EditPopupVM.AuthListItem>();

            foreach (var d in dataItems) {
                items.Add(new EditPopupVM.AuthListItem()
                {
                    ID = d.ID,
                    Code = d.AuthCode.CodeCode,
                    Description = d.AuthCode.CodeDescription
                });
            }

            return items;
        }

        internal EditPopupVM GetHoursRecord(int id) {

            var model = new Models.EditPopupVM();

            var hoursData = AppService.Current.DataContextV2.Hours.Where(x => x.ID == id).SingleOrDefault();
            model.LoadFromDomainHours(hoursData);
            model.ServiceLocationsList = GetServiceLocationsList();
            model.ServicesList = GetServicesList();

            return model;

        }

        internal ResolvePopupVM GetResolveRecord(int id)
        {
            var model = new Models.ResolvePopupVM();

            var reportData = AppService.Current.DataContextV2.HoursReportLogItems.Where(x => x.ID == id).SingleOrDefault();
            var hoursData = reportData.Hours;

            model.ID = reportData.ID;
            model.PatientName = Helpers.CommonListItems.GetCommonName(hoursData.Case.Patient.FirstName, hoursData.Case.Patient.LastName);
            model.ProviderName = Helpers.CommonListItems.GetCommonName(hoursData.Provider.FirstName, hoursData.Provider.LastName);
            model.ServiceCode = hoursData.Service.Code;
            model.ServiceName = hoursData.Service.Name;

            DateTime startTime = hoursData.Date.Add(hoursData.StartTime);
            DateTime endTime = hoursData.Date.Add(hoursData.EndTime);
            model.HoursText = hoursData.Date.ToLongDateString() + "<br/>"
                + startTime.ToString("h:mm tt") + " - " 
                + endTime.ToString("h:mm tt");

            model.ReportedBy = Helpers.CommonListItems.GetCommonName(reportData.ReportedBy.FirstName, reportData.ReportedBy.LastName);
            model.ReportedOn = reportData.DateReported;
            model.ReportedMessage = reportData.Message;
            model.ResolvedMessage = reportData.ResolvedMessage;
            model.IsResolved = true;

            return model;
        }

        public void UpdateResolveRecord(Models.ResolvePopupVM model)
        {
            var context = AppService.Current.DataContextV2;
            var reportData = context.HoursReportLogItems.Where(x => x.ID == model.ID).Single();
            var hoursData = reportData.Hours;

            reportData.ResolvedMessage = model.ResolvedMessage;
            reportData.IsResolved = model.IsResolved;

            if (model.IsResolved)
            {
                hoursData.ParentReported = false;
            }

            context.SaveChanges();

        }

        public List<ReportedListItem> GetReportedListItems()
        {
            var data = AppService.Current.DataContextV2.Hours
                .Where(x => x.ParentReported)
                .ToList();

            var items = new List<ReportedListItem>();

            foreach(var d in data)
            {
                foreach(var r in d.ReportLog)
                {

                    var item = new ReportedListItem();

                    item.CaseID = d.Case.ID;
                    item.Date = d.Date;
                    item.ID = r.ID;
                    //item.Notes = AppService.Current.DataContext.CaseAuthHoursNotes.Any(x => x.HoursID == d.ID) ? "view detail..." : d.Memo;
                    //item.Notes = d.Memo;
                    item.PatientName = Helpers.CommonListItems.GetCommonName(d.Case.Patient.FirstName, d.Case.Patient.LastName);
                    item.ProviderID = d.ProviderID;
                    item.ProviderName = Helpers.CommonListItems.GetCommonName(d.Provider.FirstName, d.Provider.LastName);
                    item.ServiceCode = d.Service?.Code;
                    item.ServiceID = d.Service?.ID;
                    item.TimeIn = d.Date + d.StartTime;
                    item.TimeOut = d.Date + d.EndTime;

                    item.ReportedByName = Helpers.CommonListItems.GetCommonName(r.ReportedBy.FirstName, r.ReportedBy.LastName);
                    item.ReportedByID = r.ReportedBy.ID;
                    item.ReportedDate = r.DateReported;
                    item.ReportedMessage = r.Message;

                    items.Add(item);

                }
            }


            return items;
        }

        public List<EditListItem> GetEditListItems(DateTime period, bool includeNonFinalized = false) {

            DateTime startDate = period;
            DateTime endDate = period.AddMonths(1).AddMilliseconds(-1);

            Domain2.Hours.HoursStatus minStatus;
            if (includeNonFinalized){ minStatus = Domain2.Hours.HoursStatus.Pending; }
            else { minStatus = Domain2.Hours.HoursStatus.FinalizedByProvider; }

            var data = AppService.Current.DataContextV2.Hours
                .Where(x => x.Date >= startDate
                && x.Date <= endDate
                && x.Status >= minStatus).ToList();

            var items = new List<EditListItem>();

            foreach (var d in data) {

                var item = new EditListItem();

                item.AuthCode = Helpers.CommonListItems.GetAuthBreakdown(d.AuthorizationBreakdowns);
                item.CaseID = d.Case.ID;
                item.Date = d.Date;
                item.ID = d.ID;
                item.Notes = d.Provider.ProviderTypeID == 15 ? "view detail..." : d.Memo;
                //item.Notes = AppService.Current.DataContext.CaseAuthHoursNotes.Any(x => x.HoursID == d.ID) ? "view detail..." : d.Memo;
                //item.Notes = d.Memo;
                item.PatientName = Helpers.CommonListItems.GetCommonName(d.Case.Patient.FirstName, d.Case.Patient.LastName);
                item.ProviderID = d.ProviderID;
                item.ProviderName = Helpers.CommonListItems.GetCommonName(d.Provider.FirstName, d.Provider.LastName);
                item.ServiceCode = d.Service?.Code;
                item.ServiceID = d.Service?.ID;
                item.TimeIn = d.Date + d.StartTime;
                item.TimeOut = d.Date + d.EndTime;
                item.Status = d.Status;
                item.Billed = d.BillingRef == null ? false : true;
                item.Paid = d.PayableRef == null ? false : true;
                item.Approved = d.ParentApproval == null ? false : true;
                item.ApprovalID = d.ParentApprovalID;
                item.HasData = d.HasCatalystData;
                item.Reported = d.ParentReported;

                items.Add(item);
                
            }

            return items;

        }

        public void ScrubSelected(List<int> selectedHours)
        {
            var context = new Data.Models.CoreEntityModel();
            foreach(int id in selectedHours)
            {
                var scrubHour = context.CaseAuthHours.Find(id);
                scrubHour.HoursStatus = (int)Domain2.Hours.HoursStatus.ScrubbedByAdmin;
            }

            context.SaveChanges();
        }

        
    }
}