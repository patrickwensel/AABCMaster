using AABC.Data.V2;
using AABC.Domain2.Cases;
using AABC.Domain2.Hours;
using AABC.DomainServices.HoursResolution;
using AABC.DomainServices.HoursResolution.Repositories;
using AABC.Shared.Web.App.HoursEntry.Models.Request;
using AABC.Shared.Web.App.HoursEntry.Models.Response;
using Dymeng.Framework.Validation;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace AABC.Shared.Web.App.HoursEntry
{
    class HoursResolutionHelper
    {

        private readonly CoreContext _context;
        // if using prepared version, these are already set
        private ResolutionService _preparedService;
        public DbContextTransaction Transaction { get; set; }


        public HoursResolutionHelper(CoreContext context)
        {
            _context = context;
        }


        public void Prepare(Hours entry, HoursEntryMode entryMode)
        {
            var repo = new ResolutionServiceRepository(_context);
            var service = new ResolutionService(new List<Hours>() { entry }, repo)
            {
                EntryMode = entryMode
            };
            _preparedService = service;
        }


        public ValidationIssueCollection GetResults(EntryApp entryApp)
        {
            // assume Prepare() has been called already
            if (_preparedService == null)
            {
                throw new InvalidOperationException("Cannot run this version of GetResults without first preparing the service");
            }
            return _preparedService.Resolve(entryApp);
        }


        public ValidationIssueCollection GetResults(Hours entry, HoursEntryMode entryMode, EntryApp entryApp, EntryType entryType, bool isPrecheck = false)
        {
            return GetResults(new List<Hours> { entry }, entryMode, entryApp, entryType, isPrecheck);
        }


        public ValidationIssueCollection GetResults(List<Hours> entries, HoursEntryMode entryMode, EntryApp entryApp, EntryType entryType, bool isPrecheck = false)
        {
            var repo = new ResolutionServiceRepository(_context);
            var service = new ResolutionService(entries, repo)
            {
                EntryMode = entryMode,
                EntryType = entryType,
                IsPreCheck = isPrecheck
            };
            return service.Resolve(entryApp);
        }


        public ValidationIssueCollection GetResults(Hours entry, HoursEntryMode entryMode, EntryApp entryApp)
        {
            return GetResults(new List<Hours>() { entry }, entryMode, entryApp, EntryType.Full);
        }


        public ValidationIssueCollection GetResults(List<Hours> entries, HoursEntryMode entryMode, EntryApp entryApp)
        {
            return GetResults(entries, entryMode, entryApp, EntryType.Full);
        }


        public Hours GetHoursObject(BaseHoursEntryRequestVM request, bool isOnAideLegacyMode)
        {
            var existingEntry = request.HoursID.HasValue ? _context.Hours.Find(request.HoursID.Value) : null;

            // the mobile app entry prechecks seem to not want to map to the V2 (probably because there's no notes
            // in the precheck).  Despite the IsOnLegacyMode flag, we're still running into some conversion errors.
            // Therefore, first try to desire mapping, and if not try the other.

            if (isOnAideLegacyMode)
            {
                try {
                    return new Mapper(_context).Map((HoursEntryRequestVM)request, existingEntry);
                } catch {
                    return new Mapper2(_context).Map((HoursEntryRequest2VM)request, existingEntry);                    
                }
            }
            else
            {
                try {
                    return new Mapper2(_context).Map((HoursEntryRequest2VM)request, existingEntry);
                } catch {
                    return new Mapper(_context).Map((HoursEntryRequestVM)request, existingEntry);
                }
                
            }
        }


        internal List<HoursEntryResponseMessage> GetResponseMessages(ValidationIssueCollection issues)
        {
            var messages = issues.Errors.Select(error => new HoursEntryResponseMessage
            {
                Message = error.Message,
                Severity = MessageSeverity.Error
            }).ToList();

            messages.AddRange(issues.Warnings.Select(warning => new HoursEntryResponseMessage
            {
                Message = warning.Message,
                Severity = MessageSeverity.Warning
            }));
            return messages;
        }


        //private Hours GetHoursObjectForAide(int caseID, int? serviceLocationID, HoursStatus status, int providerID, int serviceID,
        //    DateTime date, TimeSpan timeIn, TimeSpan timeOut, string notes, int[] ssgIDs, Hours entry, bool isTrainingEntry)
        //{
        //    if (entry == null)
        //    {
        //        entry = new Hours();
        //    }
        //    entry.CaseID = caseID;
        //    entry.ProviderID = providerID;
        //    entry.ServiceID = serviceID;
        //    entry.ServiceLocationID = serviceLocationID;
        //    entry.Date = date;
        //    entry.StartTime = timeIn;
        //    entry.EndTime = timeOut;
        //    entry.Provider = _context.Providers.Find(providerID);
        //    entry.Service = _context.Services.Find(serviceID);
        //    entry.Status = status;
        //    entry.IsTrainingEntry = isTrainingEntry;
        //    entry.Memo = notes;
        //    entry.SSGCaseIDs = ssgIDs;
        //    return entry;
        //}


        //private Hours GetHoursObjectForBCBA(int caseID, int? serviceLocationID, int providerID, HoursStatus status, int serviceID,
        //    DateTime date, TimeSpan timeIn, TimeSpan timeOut, Dictionary<int, string> extendedNotes,  // templateID, answer
        //    Hours entry, bool isTrainingEntry)
        //{
        //    if (entry == null)
        //    {
        //        entry = new Hours();
        //    }
        //    entry.CaseID = caseID;
        //    entry.ProviderID = providerID;
        //    entry.ServiceID = serviceID;
        //    entry.ServiceLocationID = serviceLocationID;
        //    entry.Date = date;
        //    entry.StartTime = timeIn;
        //    entry.EndTime = timeOut;
        //    entry.Provider = _context.Providers.Find(providerID);
        //    entry.Service = _context.Services.Find(serviceID);
        //    entry.Status = status;
        //    entry.IsTrainingEntry = isTrainingEntry;
        //    if (extendedNotes != null)
        //    {
        //        foreach (var en in extendedNotes)
        //        {
        //            var existingNote = entry.ExtendedNotes.Where(x => x.TemplateID == en.Key).SingleOrDefault();
        //            if (existingNote != null)
        //            {
        //                existingNote.Answer = en.Value;
        //            }
        //            else
        //            {
        //                entry.ExtendedNotes.Add(new ExtendedNote { TemplateID = en.Key, Answer = en.Value });
        //            }
        //        }
        //    }
        //    return entry;
        //}

    }
}