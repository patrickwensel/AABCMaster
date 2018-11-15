using AABC.Domain2.Cases;
using AABC.Domain2.Hours;
using AABC.DomainServices.HoursResolution.Repositories;
using AABC.DomainServices.Sessions;
using Dymeng.Framework.Validation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AABC.DomainServices.HoursResolution
{
    class CoreValidations
    {

        private IResolutionService _resolutionService;
        private IResolutionServiceRepository _repository;
        private readonly HoursNotesValidator _hoursNotesValidator;

        public CoreValidations(IResolutionService resolutionService, IResolutionServiceRepository repository)
        {
            _resolutionService = resolutionService;
            _repository = repository;
            _hoursNotesValidator = new HoursNotesValidator();
        }

        public bool Resolve()
        {
            foreach (var entry in _resolutionService.ProposedEntries)
            {
                if (!ValidateSSG(entry))
                {
                    return false;
                }
                if (!ValidateCoreRequirements(entry))
                {
                    return false;
                }
            }
            return true;
        }







        internal bool ValidateCoreRequirements(Domain2.Hours.Hours entry)
        {

            if (!_resolutionService.IsPreCheck)
            {
                if (entry.Date > DateTime.Now.Date)
                {
                    _resolutionService.Issues.AddIssue("Date must not be in the future.");
                    return false;
                }
            }
            else
            {
                if (entry.Date > DateTime.Now.Date.AddDays(_resolutionService.PreCheckAdvancedDaysAllowance))
                {
                    _resolutionService.Issues.AddIssue("Cannot precheck a date more than " + _resolutionService.PreCheckAdvancedDaysAllowance + " days in advance.");
                    return false;
                }
            }



            if (entry.StartTime >= entry.EndTime)
            {
                _resolutionService.Issues.AddIssue("Start Time cannot be greater than End Time");
                return false;
            }

            if (entry.Service == null)
            {
                _resolutionService.Issues.AddIssue("A Service must be supplied to the hours entry.");
                return false;
            }

            if (_resolutionService.EntryType == EntryType.Full)
            {
                var isOnAideLegacyMode = SessionReportService.IsOnAideLegacyMode(entry);
                var result = _hoursNotesValidator.Validate(entry, _resolutionService.EntryApp, isOnAideLegacyMode);
                if (!result.IsValid)
                {
                    foreach (var e in result.Errors)
                    {
                        _resolutionService.Issues.AddIssue(e);
                    }
                    return false;
                }
            }


            if (TargetPeriodFinalized(entry))
            {
                if (_resolutionService.EntryMode == HoursEntryMode.ManagementEntry)
                {
                    _resolutionService.Issues.AddIssue("The period for these hours has already been finalized", ValidationIssueType.Warning);
                }
                else
                {
                    _resolutionService.Issues.AddIssue("The period for these hours has already been finalized");
                    return false;
                }
            }


            return true;
        }


        private bool TargetPeriodFinalized(Domain2.Hours.Hours entry)
        {

            var c = _repository.GetCase(entry.CaseID);
            var period = c.GetPeriod(entry.Date.Year, entry.Date.Month);
            if (period != null)
            {
                if (period.IsProviderFinalized(entry.ProviderID))
                {
                    return true;
                }
            }
            return false;
        }


        internal bool ValidateSSG(Domain2.Hours.Hours entry)
        {

            if (entry.ServiceID != _resolutionService.SSGServiceID)
            {
                return true;
            }

            // make sure we're applying to more than one case
            if (entry.SSGCaseIDs == null || entry.SSGCaseIDs.Length < 2)
            {
                _resolutionService.Issues.AddIssue("SSG Hours must be applied to more than one case.");
                return false;
            }

            // ensure the cases we're applying to aren't finalized yet
            var finalizedTargetCases = GetFinalizedSSGTargetCases(entry);

            if (finalizedTargetCases.Count > 0)
            {
                if (_resolutionService.EntryMode == HoursEntryMode.ManagementEntry)
                {
                    _resolutionService.Issues.AddIssue("Some target cases are already finalized.", ValidationIssueType.Warning);
                }
                else
                {
                    _resolutionService.Issues.AddIssue("Some target cases are already finalized.  Unable to add SSG Hours.");
                    return false;
                }
            }

            return true;
        }

        private List<Case> GetFinalizedSSGTargetCases(Domain2.Hours.Hours entry)
        {

            var result = new List<Case>();

            var cases = _repository.GetSSGCases(entry);

            foreach (var c in cases)
            {

                var allCaseHours = c.GetAllHoursOfMonth(entry.Date);

                var hoursFinalizedByThisProvider = allCaseHours.Where(x => x.Status == HoursStatus.FinalizedByProvider && x.ProviderID == entry.ProviderID);

                if (hoursFinalizedByThisProvider.Count() > 0)
                {
                    result.Add(c);
                }
            }

            return result;
        }

    }
}
