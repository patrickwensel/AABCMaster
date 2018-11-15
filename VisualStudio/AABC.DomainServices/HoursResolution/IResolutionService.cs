using AABC.Domain2.Cases;
using AABC.Domain2.Providers;
using Dymeng.Framework.Validation;
using System.Collections.Generic;

namespace AABC.DomainServices.HoursResolution
{
    public interface IResolutionService
    {

        int SSGServiceID { get; }         // ID of the Social Skills Group service
        int DRServiceID { get; }           // ID of the DR service
        int[] AssessmentServiceIDs { get; }  // ID of the I-ASS service
        int AideProviderTypeID { get; }
        int BCBAProviderTypeID { get; }
        int PreCheckAdvancedDaysAllowance { get; set; }
        bool IsPreCheck { get; set; }

        HoursEntryMode EntryMode { get; }
        EntryType EntryType { get; }
        Provider Provider { get; }
        EntryApp EntryApp { get; }

        List<Domain2.Hours.Hours> ProposedEntries { get; }
        List<Domain2.Hours.Hours> AllProposedCaseHours(int caseID); 
        List<Domain2.Hours.Hours> AllProposedProviderHours { get; }
        
        ValidationIssueCollection Issues { get; }
        
        ValidationIssueCollection Resolve();
        ValidationIssueCollection Resolve(EntryApp entryApp);
    }
}