using AABC.Domain2.Cases;
using AABC.Domain2.Providers;
using AABC.DomainServices.HoursResolution.Logging;
using AABC.DomainServices.HoursResolution.Repositories;
using Dymeng.Framework.Validation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AABC.DomainServices.HoursResolution
{

    public enum EntryApp
    {
        Unknown = 0,
        ProviderPortal = 1,
        Manage = 2,
        ProviderApp = 3
    }

    public enum EntryType
    {
        Full = 0,   // all data is present, run a full resolution
        Basic = 1   // this is a prevalidation, notes and signatures will not be present
    }

    public class ResolutionService : IResolutionService
    {

        /* ASSUME
         *  - Single Provider per Resolution Process
         *  - Single Case per Resolution Process (adjecent SSG cases will be handled within)
         *  - One or more Hours Entries per Resolution Process
         *  - Hours will have SSG IDs filled out
         *  - Hours will have an ID of 0 for new entries, or non-zero for edits
         *  - Deletions will not be handled except in the case of SSG orphans
         *  - entry.Provider is loaded
         *  - entry.ExtendedNotes is filled, if applicable
         *  - entry.Service is loaded
         */

        /* We ran into deadlock issues running this process in production.
         * The callers to this are typically wrapped in a db transaction.
         * Within here, we want to preload what data we can outside of the transaction,
         * and only run any SaveChanges within the transaction.  As such, assume the following:
         *   PRELOAD:  safe during constructor, but do not save any entities
         *   STANDARD: safe after constructed, save any entities
         */


        // ------------------------
        // Fields and Backers
        // ------------------------
        private IResolutionServiceRepository _repository;
        private List<Domain2.Hours.Hours> _allProposedProviderHours = null;
        private Dictionary<int, List<Domain2.Hours.Hours>> _allProposedCaseHours = new Dictionary<int, List<Domain2.Hours.Hours>>();

        // ------------------------
        // Public Constants
        // ------------------------
        public int SSGServiceID { get; } = 14;                                // ID of the Social Skills Group service
        public int DRServiceID { get; } = 9;                                  // ID of the DR service

        public int AideProviderTypeID { get; } = 17;
        public int BCBAProviderTypeID { get; } = 15;

        // ------------------------
        // Public Properties
        // ------------------------
        public int[] AssessmentServiceIDs { get; set; }

        public bool UseExplicitPayableValue { get; set; } = false;      // skip standard payables calculation, use explicit entry value
        public bool UseExplicitBillableValue { get; set; } = false;     // skip standard billable calculation, use explicit entry value

        public HoursEntryMode EntryMode { get; set; } = HoursEntryMode.ProviderEntry;
        public EntryType EntryType { get; set; } = EntryType.Full;
        public Provider Provider { get; private set; }
        public EntryApp EntryApp { get; set; } = EntryApp.Unknown;

        public bool IsPreCheck { get; set; } = false;
        public int PreCheckAdvancedDaysAllowance { get; set; } = 7;

        public DateTime DateRangeLow { get; private set; }  // for proposed hours, the low and high date range
        public DateTime DateRangeHigh { get; private set; }

        // restrict all hours fetches to +/- one month from low/high range of proposed hours
        public DateTime WorkingDateRangeLow { get; private set; }
        public DateTime WorkingDateRangeHigh { get; private set; }

        public ValidationIssueCollection Issues { get; private set; } = new ValidationIssueCollection();

        public List<Domain2.Hours.Hours> ProposedEntries { get; private set; }          // all hours that are being entered
        public List<Domain2.Hours.Hours> AllProposedProviderHours { get { return GetAllProposedProviderHours(); } } // all hours for this provider, including hours being entered



        // ------------------------
        // Constructors
        // ------------------------
        public ResolutionService(IEnumerable<Domain2.Hours.Hours> entries, IResolutionServiceRepository repository)
        {

            AssessmentServiceIDs = repository.GetAssessmentIDs();

            // Warning: do not use SaveChanges in the constructor callstack.
            // See class header comments for details.

            ResolveRepository(
                repository,
                entries.Min(x => x.Date),
                entries.Max(x => x.Date));

            var firstEntry = entries.First();
            var caseID = firstEntry.CaseID;
            var providerID = firstEntry.Provider.ID;

            Provider = _repository.GetProvider(providerID);
            ProposedEntries = entries.ToList();

            // preload core provider/case hours to help prevent deadlocks
            GetAllProposedProviderHours();
            AllProposedCaseHours(caseID);

            // preload ssg case hours
            foreach (var entry in entries)
            {
                if (entry.SSGCaseIDs != null && entry.SSGCaseIDs.Length > 0)
                {
                    for (int i = 0; i < entry.SSGCaseIDs.Length; i++)
                    {
                        AllProposedCaseHours(entry.SSGCaseIDs[i]);
                    }
                }
            }

        }

        // ------------------------
        // Public Methods
        // ------------------------
        public List<Domain2.Hours.Hours> AllProposedCaseHours(int caseID)
        {

            if (_allProposedCaseHours.ContainsKey(caseID))
            {
                return _allProposedCaseHours.Where(x => x.Key == caseID).Single().Value;
            }

            var hours = _repository.GetCaseHours(caseID);

            // if any of the proposed hours have an ID, assume this is an edit and replace them, otherwise append them
            foreach (var entry in ProposedEntries)
            {
                if (entry.ID == 0)
                {
                    hours.Add(entry);
                }
                else
                {
                    var removal = hours.Where(x => x.ID == entry.ID).FirstOrDefault();
                    if (removal != null)
                    {
                        hours.Remove(hours.Where(x => x.ID == entry.ID).First());
                        hours.Add(entry);
                    }
                }
            }

            _allProposedCaseHours.Add(caseID, hours);
            return hours;
        }


        public ValidationIssueCollection Resolve()
        {
            return Resolve(EntryApp.Unknown);
        }

        public ValidationIssueCollection Resolve(EntryApp entryApp)
        {

            this.EntryApp = entryApp;

            NormalizeCoreData(entryApp);

            if (!new CoreValidations(this, _repository).Resolve())
            {
                return Issues;
            }

            // ensure all entries have IDs
            foreach (var entry in ProposedEntries)
            {
                if (entry.ID == 0)
                {
                    _repository.SaveEntry(entry);
                }
            }

            // resolves any additional SSG entries and adds them to the ProposedHours queue
            // removes any orphaned SSGs as well
            // IMPORTANT: don't reposition this, it needs to come before Case/Provider Auths
            if (!new SSGResolution(this, _repository).Resolve())
            {
                return Issues;
            }

            if (!new CaseAndProviderValidations(this).Resolve())
            {
                return Issues;
            }

            if (!new AuthorizationResolution(_repository, HoursResolutionLoggingService.Create()).Resolve(ProposedEntries))
            {
                return Issues;
            }

            if (!new ScheduleResolution(this).Resolve())
            {
                return Issues;
            }

            return Issues;
        }

        public void NormalizeCoreData(EntryApp entryApp)
        {
            foreach (var entry in ProposedEntries)
            {
                entry.Date = entry.Date.Date;   // trim time from date
                entry.EntryApp = GetEntryAppString(entryApp);
                entry.TotalHours = (decimal)(entry.EndTime - entry.StartTime).TotalHours;

                if (!UseExplicitPayableValue)
                {
                    entry.PayableHours = entry.TotalHours;
                }

                if (!UseExplicitBillableValue)
                {
                    if (entry.IsTrainingEntry)
                    {
                        entry.BillableHours = 0;
                    }
                    else
                    {
                        entry.BillableHours = entry.TotalHours;
                    }
                }



            }
        }


        // ------------------------
        // Private Methods
        // ------------------------


        private void ResolveRepository(IResolutionServiceRepository repository, DateTime dateRangeLow, DateTime dateRangeHigh)
        {
            if (repository == null)
            {
                _repository = new ResolutionServiceRepository(new Data.V2.CoreContext());
            }
            else
            {
                _repository = repository;
            }

            // set the effective repository +/- one month from proposed hours range
            _repository.DateRangeLow = new DateTime(dateRangeLow.Year, dateRangeLow.Month, 1).AddMonths(-1);
            _repository.DateRangeHigh = new DateTime(dateRangeHigh.Year, dateRangeHigh.Month, 1).AddMonths(1);
        }

        private List<Domain2.Hours.Hours> GetAllProposedProviderHours()
        {

            if (_allProposedProviderHours != null)
            {
                return _allProposedProviderHours;
            }

            var hours = _repository.GetProviderHours(this.Provider.ID);

            // if any of the proposed hours have an ID, assume this is an edit and replace them, otherwise append them
            foreach (var entry in ProposedEntries)
            {
                if (entry.ID == 0)
                {
                    hours.Add(entry);
                }
                else
                {
                    hours.Remove(hours.Where(x => x.ID == entry.ID).First());
                    hours.Add(entry);
                }
            }

            _allProposedProviderHours = hours;
            return hours;
        }



        private string GetEntryAppString(EntryApp value)
        {
            switch (value)
            {
                case EntryApp.Unknown: return null;
                case EntryApp.Manage: return "Manage";
                case EntryApp.ProviderApp: return "Provider App";
                case EntryApp.ProviderPortal: return "Provider Portal";
                default: return null;
            }
        }

    }

}
