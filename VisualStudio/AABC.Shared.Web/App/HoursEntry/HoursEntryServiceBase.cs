using AABC.Data.V2;
using AABC.Domain2.Cases;
using AABC.Domain2.Hours;
using AABC.Domain2.Providers;
using AABC.Domain2.Services;
using AABC.DomainServices.Hours;
using AABC.DomainServices.HoursResolution;
using AABC.DomainServices.Providers;
using AABC.DomainServices.Services;
using AABC.DomainServices.Sessions;
using AABC.Shared.Web.App.HoursEntry.Models;
using AABC.Shared.Web.App.HoursEntry.Models.Request;
using AABC.Shared.Web.App.HoursEntry.Models.Response;
using Dymeng.Framework;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;


namespace AABC.Shared.Web.App.HoursEntry
{
    public abstract class HoursEntryServiceBase
    {
        private readonly HoursEntryMode EntryMode;
        private readonly SessionReportService SessionReportService;
        protected CoreContext Context { get; private set; }

        protected HoursEntryServiceBase(CoreContext context, HoursEntryMode entryMode = HoursEntryMode.ProviderEntry)// default to more restricted of modes
        {
            Context = context;
            SessionReportService = new SessionReportService(Context);
            EntryMode = entryMode;
        }


        internal IEnumerable<HoursEntryServiceVM> GetServices(int caseID, int providerID, DateTime date)
        {
            var @case = Context.Cases.Find(caseID);
            var provider = Context.Providers.Find(providerID);
            var sp = new ServiceProvider(Context);
            var services = sp.GetServices(@case, provider, date);
            var items = services.Select(m => HoursEntryServiceVM.MapFromDomain(m));
            return items;
        }



        public HoursEntryResponseVM SubmitHoursForProviderAppManualEntryInitialValidation(BaseHoursEntryRequestVM request, bool isOnAideLegacyMode)
        {
            // this runs when the mobile app is pre-validating a manual entry
            // there are no notes present: we're validating the base information only
            // we are not saving anything
            var resolutionDataContext = new CoreContext();
            using (var transaction = resolutionDataContext.Database.BeginTransaction())
            {
                var helper = new HoursResolutionHelper(resolutionDataContext);
                try
                {
                    var entry = helper.GetHoursObject(request, isOnAideLegacyMode);
                    var issues = helper.GetResults(entry, HoursEntryMode.ProviderEntry, EntryApp.ProviderApp, EntryType.Basic);
                    var response = new HoursEntryResponseVM
                    {
                        WasProcessed = false
                    };
                    if (issues.HasErrors || issues.HasWarnings)
                    {
                        response.Messages = helper.GetResponseMessages(issues);
                    }
                    return response;
                }
                catch (Exception e)
                {
                    Exceptions.Handle(e);
                    return new HoursEntryResponseVM
                    {
                        HoursID = request.HoursID,
                        WasProcessed = false,
                        Messages = new List<HoursEntryResponseMessage> {
                            new HoursEntryResponseMessage {
                                Severity = MessageSeverity.Error,
                                Message = "We're sorry, but an unknown error has occurred.  Please contact your system administrator for more information."
                            }
                        }
                    };
                }
                finally
                {
                    // we're just validating, rollback
                    Rollback(transaction);
                }
            }
        }


        public HoursEntryResponseVM SubmitHoursForProviderAppPreCheck(BaseHoursEntryRequestVM request, bool isOnAideLegacyMode)
        {
            // this runs when the mobile app is setting it's precheck
            // there are no notes present: we're validating the base information only
            // we are saving if the base information is valid
            var resolutionDataContext = new CoreContext();
            using (var transaction = resolutionDataContext.Database.BeginTransaction())
            {
                var helper = new HoursResolutionHelper(resolutionDataContext);
                try
                {
                    var entry = helper.GetHoursObject(request, isOnAideLegacyMode);
                    var issues = helper.GetResults(entry, HoursEntryMode.ProviderEntry, EntryApp.ProviderApp, EntryType.Basic, true);
                    if (issues.HasErrors || (issues.HasWarnings && !request.IgnoreWarnings.GetValueOrDefault()))
                    {
                        Rollback(transaction);
                        return new HoursEntryResponseVM
                    {
                            HoursID = entry.ID,
                            WasProcessed = false,
                            Messages = helper.GetResponseMessages(issues)
                        };
                    }

                    resolutionDataContext.SaveChanges();
                    transaction.Commit();
                    return new HoursEntryResponseVM
                    {
                        HoursID = entry.ID,
                        WasProcessed = true
                    };
                }
                catch (Exception e)
                {
                    Rollback(transaction);
                    Exceptions.Handle(e);
                    return new HoursEntryResponseVM
                    {
                        HoursID = request.HoursID,
                        WasProcessed = false,
                        Messages = new List<HoursEntryResponseMessage> {
                            new HoursEntryResponseMessage {
                                Severity = MessageSeverity.Error,
                                Message = "We're sorry, but an unknown error has occurred.  Please contact your system administrator for more information. (ExecInf:" + e.ToString() + ");"
                            }
                        }
                    };
                }
            }
        }



        public HoursEntryResponseVM SubmitHoursForValidation(BaseHoursEntryRequestVM request, EntryApp entryApp, bool isOnAideLegacyMode)
        {
            // this runs when an application is validating a full entry
            // all information is present
            // nothing is saved: this is for validation only
            var resolutionDataContext = new CoreContext();
            using (var transaction = resolutionDataContext.Database.BeginTransaction())
            {
                var helper = new HoursResolutionHelper(resolutionDataContext);
                try
                {
                    var entry = helper.GetHoursObject(request, isOnAideLegacyMode);
                    var issues = helper.GetResults(entry, EntryMode, entryApp, EntryType.Full);

                    if (issues.HasErrors || (issues.HasWarnings && !request.IgnoreWarnings.GetValueOrDefault()))
                    {
                        Rollback(transaction);
                        return new HoursEntryResponseVM
                        {
                            HoursID = request.HoursID,
                            WasProcessed = false,
                            Messages = helper.GetResponseMessages(issues)
                    };
                    }
                    return new HoursEntryResponseVM
                    {
                        HoursID = request.HoursID
                    };
                    }
                catch (Exception e)
                {
                    Exceptions.Handle(e);
                    return new HoursEntryResponseVM
                    {
                        HoursID = request.HoursID,
                        WasProcessed = false,
                        Messages = new List<HoursEntryResponseMessage> {
                            new HoursEntryResponseMessage {
                                Severity = MessageSeverity.Error,
                                Message = "We're sorry, but an unknown error has occurred.  Please contact your system administrator for more information."
                            }
                        }
                    };
                }
                finally
                {
                    // we're just validating, don't actually submit all of these changes
                    Rollback(transaction);
                }
            }
        }


        public HoursEntryResponseVM SubmitHoursRequest(BaseHoursEntryRequestVM request, EntryApp entryApp, bool isOnAideLegacyMode)
        {
            // this runs when an application is saving a full entry
            // all information is present
            // validation should have been done already, but we run again for good measure
            // this entry is saved to the database if valid
            if (EntryMode == HoursEntryMode.ManagementEntry)
            {
                request.IgnoreWarnings = true;
            }

            var resolutionDataContext = new CoreContext();
            var helper = new HoursResolutionHelper(resolutionDataContext);
            var entry = helper.GetHoursObject(request, isOnAideLegacyMode);
            helper.Prepare(entry, EntryMode);

            using (var transaction = resolutionDataContext.Database.BeginTransaction())
            {
                helper.Transaction = transaction;
                try
                {
                    var issues = helper.GetResults(entryApp);
                    if (issues.HasErrors || (issues.HasWarnings && !request.IgnoreWarnings.GetValueOrDefault()))
                    {
                        Rollback(transaction);
                        return new HoursEntryResponseVM
                    {
                            HoursID = request.HoursID,
                            WasProcessed = false,
                            Messages = helper.GetResponseMessages(issues)
                        };
                    }

                    var signatureType = "image/png";
                    if (request.HasSignatures) {
                        entry.SessionSignature = new SessionSignature
                        {
                        ProviderSignature = request.Signatures[0].Base64Signature,
                        ProviderName = request.Signatures[0].Name,
							ProviderSignatureType = signatureType,
                        ParentSignature = request.Signatures[1].Base64Signature,
                        ParentName = request.Signatures[1].Name,
							ParentSignatureType = signatureType
						};
                    }                    


                    if (request.AllowHasDataChanges)
                    {
                        entry.HasCatalystData = request.HasData;
                    }
                    if (request.CatalystPreloadID.HasValue)
                    {
                        entry.HasCatalystData = true;
                        var preload = resolutionDataContext.CatalystPreloadEntries.Find(request.CatalystPreloadID.Value);
                        if (preload != null)
                        {
                            preload.IsResolved = true;
                        }
                    }
                    resolutionDataContext.SaveChanges();
                    transaction.Commit();
                    return new HoursEntryResponseVM
                    {
                        HoursID = request.HoursID,
                        WasProcessed = true,
                    };
                }
                catch (Exception e)
                {
                    Rollback(transaction);
                    Exceptions.Handle(e);
                    return new HoursEntryResponseVM
                    {
                        HoursID = request.HoursID,
                        WasProcessed = false,
                        Messages = new List<HoursEntryResponseMessage> {
                            new HoursEntryResponseMessage {
                                Severity = MessageSeverity.Error,
                                Message = "We're sorry, but an unknown error has occurred.  Please contact your system administrator for more information."
                            }
                        }
                    };
                }
            }
        }


        private void Rollback(DbContextTransaction transaction)
        {
            try
            {
                transaction.Rollback();
            }
            catch (Exception e)
            {
                Exceptions.Handle(e);
            }
        }



        /// <summary>
        /// Determine if we can edit an existing hours entry.  If in ProviderEntry mode, pass the editing provider ID
        /// </summary>
        public bool GetEditEligibility(int hoursID, int? editingByProviderID, out string nonEditableReason)
        {
            nonEditableReason = null;
            if (hoursID < 0)
            {
                // this is a catalyst preentry
                return true;
            }
            var entry = Context.Hours.Find(hoursID);
            if (EntryMode == HoursEntryMode.ProviderEntry)
            {
                // if we're finalized, don't allow any edits
                var period = entry.Case.GetPeriod(entry.Date.Year, entry.Date.Month);
                if (period != null)
                {
                    if (period.IsProviderFinalized(entry.ProviderID))
                    {
                        nonEditableReason = "Unable to edit this entry, period has been finalized.";
                        return false;
                    }
                }
            }

            // if this is a provider edit, make sure the provider owns these hours
            if (EntryMode == HoursEntryMode.ProviderEntry)
            {
                if (editingByProviderID.GetValueOrDefault(0) == entry.ProviderID)
                {
                    nonEditableReason = "Unable to edit this entry, as you were not the provider that originally entered them.";
                    return false;
                }
            }

            // if this is an SSG entry, make sure it's the parent entry
            if (entry.SSGParentID.HasValue)
            {
                if (entry.SSGParentID.Value != entry.ID)
                {
                    nonEditableReason = "Unable to edit: SSG entries must be edited by the entry that create it.";
                    return false;
                }
            }
            // passed all
            return true;
        }


        public HoursEntryVM GetNewHoursEntryDataTemplate(int caseID, int providerID, DateTime? date)
        {
            var provider = Context.Providers.Find(providerID);
            var c = Context.Cases.Find(caseID);
            var model = new HoursEntryVM
            {
                IsOnAideLegacyMode = SessionReportService.IsOnAideLegacyMode(provider),
                IsEditable = true, // this always has to be true, because a date won't be selected until later via the UI                
                Status = (int)HoursStatus.Pending,
                HasData = false,
                CaseID = c.ID,
                IsAdminMode = EntryMode == HoursEntryMode.ManagementEntry,
                EntryID = null,
                ProviderID = providerID,
                ProviderTypeID = provider.ProviderTypeID,
                ProviderTypeCode = provider.ProviderType.Code,
                PatientID = c.PatientID,
                PatientName = c.Patient.CommonName,
                InsuranceID = null,
                InsuranceName = "Select a date to determine the active insurance",
                Date = date,
                TimeIn = null,
                TimeOut = null,
                Note = null,
                ServiceID = null,
                ServiceLocations = GetServiceLocations(),
                IsTrainingEntry = false,
                ActivePatients = new List<HoursEntryActivePatientVM>(),   // can't get these before we have a date on file...
                NoteGroups = GetNewHoursEntryNoteGroups(provider.ProviderTypeID),
                Services = date.HasValue ? GetHoursEntryServices(c, provider, date.Value) : new List<HoursEntryServiceVM>(),
                SessionReport = new DomainServices.Sessions.SessionReport()
            };
            model.ServiceLocationID = model.ServiceLocations.Where(x => x.Name == "Home").FirstOrDefault()?.ID;
            return model;
        }


        public Domain2.Insurances.Insurance GetActiveInsurance(int patientID, DateTime date)
        {
            var c = Context.Cases.Where(x => x.PatientID == patientID).SingleOrDefault();
            return c?.GetActiveInsuranceAtDate(date)?.Insurance;
        }


        public void DeleteHours(int hoursID)
        {
            HoursRemovalService.DeleteHours(hoursID, Context);
        }


        public string GetDeleteConfirmSummary(int hoursID)
        {
            var entry = Context.Hours.Find(hoursID);
            return $"{entry.Date.ToShortDateString()} - {entry.Case.Patient.CommonName} ({entry.Service.Name})";
        }


        public IEnumerable<ProviderSelectListItem> GetProviderSelectModel(int caseID)
        {
            var items = Context.Cases.Find(caseID).Providers.ToList().Select(cp => ProviderSelectListItem.Transform(cp.Provider));
            return items;
        }


        /// <summary>
        /// If this edit request is via provider, pass the provider's id to editingByProviderID
        /// </summary>
        /// <param name="hoursID"></param>
        /// <param name="editingByProviderID"></param>
        /// <returns></returns>
        public HoursEntryVM GetHoursEntryVM(int hoursID, int? editingByProviderID)
        {
            var data = Context.Hours.Find(hoursID);
            var model = new HoursEntryVM
            {
                IsOnAideLegacyMode = SessionReportService.IsOnAideLegacyMode(data),
                IsAdminMode = EntryMode == HoursEntryMode.ManagementEntry,
                IsEditable = GetEditEligibility(hoursID, editingByProviderID, out string nonEditableReason),
                NonEditableReason = nonEditableReason,
                Status = (int)data.Status,
                HasData = data.HasCatalystData,
                EntryID = data.ID,
                ProviderID = data.ProviderID,
                ProviderTypeID = data.Provider.ProviderTypeID,
                ProviderTypeCode = data.Provider.ProviderType.Code,
                PatientID = data.Case.Patient.ID,
                CaseID = data.Case.ID,
                PatientName = data.Case.Patient.CommonName,
                InsuranceID = data.Case.GetActiveInsuranceAtDate(data.Date)?.InsuranceID,
                InsuranceName = data.Case.GetActiveInsuranceAtDate(data.Date)?.Insurance.Name,
                Date = data.Date,
                TimeIn = data.Date + data.StartTime,
                TimeOut = data.Date + data.EndTime,
                Note = data.Memo,
                SSGCaseIDs = SocialSkillsGroup.GetAssociatedCaseIDs(hoursID),
                ServiceID = data.ServiceID.Value,
                ServiceLocationID = data.ServiceLocationID,
                IsTrainingEntry = data.IsTrainingEntry,
                Services = GetHoursEntryServices(data.Case, data.Provider, data.Date),
                ServiceLocations = GetServiceLocations(),
                ActivePatients = GetHoursEntryActivePatients(data.ProviderID, data.Date),
                NoteGroups = GetHoursEntryNoteGroups(hoursID),
                IsNonParentSSGEntry = data.ServiceID == (int)ServiceIDs.SocialSkillsGroup && !SocialSkillsGroup.IsOriginatingHoursEntry(hoursID),
                SessionReport = data.Report != null && !string.IsNullOrEmpty(data.Report.Report) ? JsonConvert.DeserializeObject<DomainServices.Sessions.SessionReport>(data.Report.Report) : new DomainServices.Sessions.SessionReport()
            };
            return model;
        }


        public DomainServices.Sessions.SessionReportConfiguration GetSessionReportConfig(ProviderTypeIDs providerTypeID, int serviceID)
        {
            return SessionReportService.GetSessionReportConfig(providerTypeID, serviceID);
        }


        public IEnumerable<ServiceLocationListItem> GetServiceLocations()
        {
            var data = Context.ServiceLocations.Where(x => x.Active).OrderBy(x => x.Name).ToList();
            var items = data.Select(d => new ServiceLocationListItem
            {
                Name = d.Name,
                ID = d.ID
            });
            return items;
        }



        public IEnumerable<HoursEntryServiceVM> GetHoursEntryServices(Case @case, Provider provider, DateTime refDate)
        {
            var sp = new ServiceProvider(Context);
            var services = sp.GetServices(@case, provider, refDate);
            var items = services.Select(m => HoursEntryServiceVM.MapFromDomain(m));
            return items;
        }


        [Obsolete("Use insurance-driven service fetching instead of legacy service fetching")]
        public IEnumerable<HoursEntryServiceVM> GetHoursEntryServices(ProviderTypeIDs providerTypeID)
        {
            var sp = new ServiceProvider(Context);
            var services = sp.GetLegacyServicesByProviderType(providerTypeID);
            var items = services.Select(m => HoursEntryServiceVM.MapFromDomain(m));
            return items;
        }


        public IEnumerable<HoursEntryActivePatientVM> GetHoursEntryActivePatients(int providerID, DateTime refDate)
        {
            var activeCases = Context.Providers.Find(providerID).GetActiveCasesAtDate(refDate);
            var items = activeCases.Select(c => new HoursEntryActivePatientVM
            {
                CaseID = c.ID,
                Name = c.Patient.CommonName,
                PatientID = c.PatientID
            });
            return items;
        }


        public IEnumerable<HoursEntryNoteGroupVM> GetNewHoursEntryNoteGroups(int providerTypeID)
        {
            var notes = ExtendedNotesTemplateResolver.GetExtendedNotesBlankMatrix((ProviderTypeIDs)providerTypeID);
            var groups = notes.Select(x => x.Template).Select(x => x.Group).Distinct().ToList();
            var results = groups.Select(m => new HoursEntryNoteGroupVM
            {
                ID = m.ID,
                Name = m.Name,
                Notes = notes.Where(x => x.Template.GroupID == m.ID)
                             .OrderBy(x => x.Template.DisplaySequence)
                             .ToList()
                             .Select(note => new HoursEntryNoteVM
                             {
                                 Answer = note.Answer,
                                 ID = note.ID,
                                 Question = note.Template.Text,
                                 TemplateID = note.TemplateID
                             }).ToList()
            });
            return results;
        }


        public IEnumerable<HoursEntryNoteGroupVM> GetHoursEntryNoteGroups(int hoursID)
        {
            var notes = ExtendedNotesTemplateResolver.GetExtendedNotesMatrix(hoursID);
            var allTemplates = notes.Select(x => x.Template).ToList();
            var allGroups = allTemplates.Select(x => x.Group).ToList();
            var groups = allGroups.Distinct().ToList();
            var results = groups.Select(m => new HoursEntryNoteGroupVM
            {
                ID = m.ID,
                Name = m.Name,
                Notes = notes.Where(x => x.Template.GroupID == m.ID)
                 .OrderBy(x => x.Template.DisplaySequence)
                 .ToList()
                 .Select(note => new HoursEntryNoteVM
                 {
                     Answer = note.Answer,
                     ID = note.ID,
                     Question = note.Template.Text,
                     TemplateID = note.TemplateID
                 }).ToList()
            });
            return results;
        }


    }
}
