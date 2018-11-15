using AABC.Domain2.Cases;
using AABC.Domain2.Hours;
using AABC.Domain2.Providers;
using System;
using System.Collections.Generic;

namespace AABC.DomainServices.HoursResolution.Repositories
{
    public interface IResolutionServiceRepository
    {

        Case GetCase(int caseID);
        Provider GetProvider(int providerID);
        List<Domain2.Hours.Hours> GetCaseHours(int caseID);
        List<Domain2.Hours.Hours> GetProviderHours(int providerID);
        List<Case> GetSSGCases(Domain2.Hours.Hours entry);
        Domain2.Insurances.Insurance GetActiveInsurance(int caseID, DateTime refDate);

        DateTime DateRangeLow { get; set; }
        DateTime DateRangeHigh { get; set; }

        void RemoveAuthorizationBreakdown(AuthorizationBreakdown e);

        int? GetPriorEntryServiceID(Domain2.Hours.Hours entry);
        void RemoveOrphanedSSGRecords(int ssgParentID);
        void AddSSGEntry(Domain2.Hours.Hours newEntry);
        void SaveEntry(Domain2.Hours.Hours entry);
        int[] GetAssessmentIDs();
    }
}
