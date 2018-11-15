using System;

namespace AABC.DomainServices.Hours
{
    public class HourEx
    {

        public int Id { get; set; }
        public DateTime DateCreated { get; set; }
        public int? AuthorizationID { get; set; }
        public int CaseID { get; set; }
        public string PatientName { get; set; }
        public int ProviderID { get; set; }
        public string ProviderName { get; set; }
        public string Status { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public int? ServiceID { get; set; }
        public string Memo { get; set; }
        public decimal TotalHours { get; set; }
        public decimal BillableHours { get; set; }
        public decimal PayableHours { get; set; }
        public string BillingRef { get; set; }
        public string PayableRef { get; set; }
        public bool HasCatalystData { get; set; }
        public bool WatchEnabled { get; set; }
        public string WatchNote { get; set; }
        public int? SSGParentID { get; set; }
        public int? CorrelationID { get; set; }
        public string InternalMemo { get; set; }
        public bool IsAdjustment { get; set; }
        public int? ServiceLocationID { get; set; }
        public int? ParentApprovalID { get; set; }
        public bool ParentReported { get; set; }
    }
}