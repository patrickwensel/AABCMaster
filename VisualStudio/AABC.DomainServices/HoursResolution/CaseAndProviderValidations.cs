using AABC.Domain2.Cases;
using Dymeng.Framework.Validation;
using System;
using System.Collections.Generic;
using System.Linq;



namespace AABC.DomainServices.HoursResolution
{

    internal class CaseAndProviderValidations
    {
        private IResolutionService _resolutionService;
        private int[] _assessmentServiceIDs;

        public CaseAndProviderValidations(IResolutionService resolutionService)
        {
            _resolutionService = resolutionService;
            _assessmentServiceIDs = _resolutionService.AssessmentServiceIDs;
        }

        public bool Resolve()
        {

            foreach (var entry in _resolutionService.ProposedEntries)
            {

                if (!ValidateProviderNotSelfOverlap(entry))
                {
                    return false;
                }

                if (!ValidateAideNotDROverlapOnCase(entry))
                {
                    return false;
                }

                if (!ValidateAideMaxHoursPerDayPerCase(entry))
                {
                    return false;
                }

                if (!ValidateBCBAMaxHoursPerEntry(entry))
                {
                    return false;
                }

                if (!ValidateBCBAMaxAssessmentHoursPerCasePerDay(entry))
                {
                    return false;
                }
            }
            return true;
        }



        internal bool ValidateBCBAMaxAssessmentHoursPerCasePerDay(Domain2.Hours.Hours entry)
        {
            const int MAX_ASSESSMENT_HOURS = 6;
            if (!_resolutionService.Provider.IsBCBA || (_resolutionService.EntryMode == HoursEntryMode.ManagementEntry && entry.IsTrainingEntry) || (!_assessmentServiceIDs.ToList().Contains(entry.Service.ID)))
            {
                return true;
            }

            var caseHoursToday = _resolutionService.AllProposedCaseHours(entry.CaseID)
                .Where(x => x.Date == entry.Date && x.Provider.ID == _resolutionService.Provider.ID && _assessmentServiceIDs.ToList().Contains(x.Service.ID))
                .ToList();
            if (caseHoursToday.Sum(x => x.TotalHours) > MAX_ASSESSMENT_HOURS)
            {

                if (_resolutionService.EntryMode == HoursEntryMode.ManagementEntry)
                {
                    _resolutionService.Issues.AddIssue($"BCBA Assessment Hours over max {MAX_ASSESSMENT_HOURS} per case per day.", ValidationIssueType.Warning);
                }
                else
                {
                    _resolutionService.Issues.AddIssue($"Assessment Hours over max {MAX_ASSESSMENT_HOURS} per case per day.");
                    return false;
                }
            }
            return true;
        }


        internal bool ValidateBCBAMaxHoursPerEntry(Domain2.Hours.Hours entry)
        {
            const int MAX_NON_ASSESSMENT_HOURS = 2;
            if (!_resolutionService.Provider.IsBCBA || (_assessmentServiceIDs.ToList().Contains(entry.Service.ID)))
            {
                return true;
            }

            if (entry.TotalHours > MAX_NON_ASSESSMENT_HOURS)
            {
                if (_resolutionService.EntryMode == HoursEntryMode.ManagementEntry)
                {
                    _resolutionService.Issues.AddIssue($"Maximum {MAX_NON_ASSESSMENT_HOURS} per day for non-assessment services.", ValidationIssueType.Warning);
                }
                else
                {
                    _resolutionService.Issues.AddIssue($"Unable to add hours, maximum {MAX_NON_ASSESSMENT_HOURS} per day for non-assessment services.");
                    return false;
                }
            }
            return true;
        }


        internal bool ValidateAideMaxHoursPerDayPerCase(Domain2.Hours.Hours entry)
        {
            const int MAX_HOURS = 4;
            if (!_resolutionService.Provider.IsAide || (_resolutionService.EntryMode == HoursEntryMode.ManagementEntry && entry.IsTrainingEntry))
            {
                return true;
            }

            var caseHoursPerDate = _resolutionService.AllProposedCaseHours(entry.CaseID).Where(x => x.Date == entry.Date && x.ProviderID == entry.ProviderID).ToList();
            var sumOfHours = caseHoursPerDate.Sum(x => x.TotalHours);
            if (sumOfHours > MAX_HOURS)
            {
                if (_resolutionService.EntryMode == HoursEntryMode.ManagementEntry)
                {
                    _resolutionService.Issues.AddIssue("This entry will submit more than the max hours per day per case for this provider", ValidationIssueType.Warning);
                }
                else
                {
                    _resolutionService.Issues.AddIssue("Unable to submit, this will cause your max hours per day, per case to be over.");
                    return false;
                }
            }
            return true;
        }


        internal bool ValidateAideNotDROverlapOnCase(Domain2.Hours.Hours entry)
        {
            if (_resolutionService.Provider.IsBCBA || (_resolutionService.EntryMode == HoursEntryMode.ManagementEntry && entry.IsTrainingEntry))
            {
                return true;
            }

            var caseDRHours = _resolutionService.AllProposedCaseHours(entry.CaseID)
                .Where(x => x.Date == entry.Date && x.ServiceID == _resolutionService.DRServiceID)
                .ToList();
            var tuples = GetHoursRangeTuples(caseDRHours);

            if (ValidationHelpers.AreAnyOverlapping(tuples))
            {
                if (_resolutionService.EntryMode == HoursEntryMode.ManagementEntry)
                {
                    _resolutionService.Issues.AddIssue("These hours will cause an overlap in Aide DR entries for this case.", ValidationIssueType.Warning);
                }
                else
                {
                    _resolutionService.Issues.AddIssue("Unable to apply hours, this would cause an overlap of DR services for this case.");
                    return false;
                }
            }
            return true;
        }


        internal bool ValidateProviderNotSelfOverlap(Domain2.Hours.Hours entry)
        {
            if (_resolutionService.Provider.IsBCBA)
            {
                return true;
            }

            var hoursAtDate = _resolutionService.AllProposedProviderHours.Where(x => x.Date == entry.Date).ToList();
            // remove any of these that are SSG non-parents
            var ssgToRemove = hoursAtDate.Where(x => x.SSGParentID != null && x.SSGParentID != x.ID).ToList();
            foreach (var removal in ssgToRemove)
            {
                hoursAtDate.Remove(removal);
            }

            var tuples = GetHoursRangeTuples(hoursAtDate);
            if (ValidationHelpers.AreAnyOverlapping(tuples))
            {
                if (_resolutionService.EntryMode == HoursEntryMode.ManagementEntry)
                {
                    _resolutionService.Issues.AddIssue("These hours cause an overlap by this provider.", ValidationIssueType.Warning);
                }
                else
                {
                    _resolutionService.Issues.AddIssue("Unable to apply hours, this would cause a time overlap of other entries.");
                    return false;
                }
            }
            return true;
        }


        private Tuple<DateTime, DateTime>[] GetHoursRangeTuples(IEnumerable<Domain2.Hours.Hours> hours)
        {
            return hours.Select(h => new Tuple<DateTime, DateTime>(h.Date + h.StartTime, h.Date + h.EndTime)).ToArray();
        }

    }
}
