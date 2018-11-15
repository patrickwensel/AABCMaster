using System;

namespace AABC.DomainServices.HoursResolution.Logging
{
    public class HoursResolutionLoggingEntry
    {
        public bool WasResolved { get; set; }
        public int HoursID { get; set; }
        public DateTime HoursDate { get; set; }
        public int ServiceID { get; set; }
        public decimal BillableHours { get; set; }
        public int ProviderTypeID { get; set; }
        public int InsuranceID { get; set; }
        public string AuthMatchRuleDetailJSON { get; set; }
        public string ActiveAuthorizationsJSON { get; set; }
        public int? ResolvedAuthID { get; set; }
        public string ResolvedAuthCode { get; set; }
        public int? ResolvedCaseAuthID { get; set; }
        public DateTime? ResolvedCaseAuthStart { get; set; }
        public DateTime? ResolvedCaseAuthEndDate { get; set; }
        public int? ResolvedAuthMatchRuleID { get; set; }
        public int? ResolvedMinutes { get; set; }
    }
}
