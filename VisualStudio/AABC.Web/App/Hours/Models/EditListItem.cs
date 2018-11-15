using System;

namespace AABC.Web.App.Hours.Models
{
    public class EditListItem
    {

        public int ID { get; set; }

        public Domain2.Hours.HoursStatus Status { get; set; }

        public string StatusText {
            get
            {
                switch (Status) {
                    case Domain2.Hours.HoursStatus.ComittedByProvider:
                        return "Committed";
                    case Domain2.Hours.HoursStatus.FinalizedByProvider:
                        return "Finalized";
                    case Domain2.Hours.HoursStatus.Pending:
                        return "Entered";
                    case Domain2.Hours.HoursStatus.ProcessedComplete:
                        return "Completed";
                    case Domain2.Hours.HoursStatus.ScrubbedByAdmin:
                        return "Scrubbed";
                    default:
                        return "Unknown";
                }
            }
        }

        public int CaseID { get; set; }
        public string PatientName { get; set; }

        public int ProviderID { get; set; }
        public string ProviderName { get; set; }

        public DateTime Date { get; set; }
        public DateTime TimeIn { get; set; }
        public DateTime TimeOut { get; set; }

        public int? ServiceID { get; set; }
        public string ServiceCode { get; set; }

        public string AuthCode { get; set; }
        
        public string Notes { get; set; }

        public bool Billed { get; set; }
        public bool Paid { get; set; }
        public bool HasData { get; set; }
        public bool Approved { get; set; }
        public int? ApprovalID { get; set; }
        public bool Reported { get; set; }


    }
}