using System;
using System.Collections.Generic;

namespace AABC.Domain.Cases
{
    public class CaseAuthorizationHours
    {
        public int? ID { get; set; }
        public DateTime? DateCreated { get; set; }

        public AuthorizationHoursStatus Status { get; set; }
        public int? CaseID { get; set; }

        public CaseAuthorization Authorization { get; set; }

        public CaseProvider Provider { get; set; }
        public int ProviderID { get; set; }
        public DateTime Date { get; set; }
        public DateTime TimeIn { get; set; }
        public DateTime TimeOut { get; set; }
        public double HoursTotal { get; set; }
        public double? BillableHours { get; set; }
        public double? PayableHours { get; set; }
        public Service Service { get; set; }
        public string Notes { get; set; }
        public bool HasExtendedNotes { get; set; }
        public Dictionary<string, List<Domain.Hours.Note>> ExtendedNotes { get; set; }
        public string BillingRef { get; set; }
        public string PayableRef { get; set; }
        public bool HasCatalystData { get; set; }

        public int? SSGParentID { get; set; }   // if this service is a Social Skills Group service,
        public bool WatchEnabled { get; set; }
        public string WatchNote { get; set; }
        public int? CorrelationID { get; set; }
        public string InternalNotes { get; set; }
        public bool IsPayrollOrBillingAdjustment { get; set; }
        public int? ServiceLocationID { get; set; }

        public List<Hours.Note> MultiNotes { get; set; }

        public string ServiceCode {
            get
            {
                return Service?.Code;
            }
        }

        public Domain.Cases.Case Case { get; set; }
        public Domain.Services.ServiceLocation ServiceLocation { get; set; }

    }
}
