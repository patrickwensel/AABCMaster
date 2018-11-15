using System;

namespace AABC.Web.App.Hours.Models
{
    public class ResolvePopupVM
    {
        public int ID { get; set; }
        public string PatientName { get; set; }
        public string ProviderName { get; set; }
        public string ServiceCode { get; set; }
        public string ServiceName { get; set; }
        public string HoursText { get; set; }

        public DateTime Date { get; set; }
        public DateTime TimeIn { get; set; }
        public DateTime TimeOut { get; set; }
        public decimal TotalHours { get; set; }

        public string ReportedBy { get; set; }
        public DateTime ReportedOn { get; set; }
        public string ReportedMessage { get; set; }
        public string ResolvedMessage { get; set; }
        public bool IsResolved { get; set; }


    }
}