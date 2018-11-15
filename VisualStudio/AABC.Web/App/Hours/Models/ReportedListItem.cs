using System;

namespace AABC.Web.App.Hours.Models
{
    public class ReportedListItem
    {

        public int ID { get; set; }

        public int CaseID { get; set; }
        public string PatientName { get; set; }

        public int ProviderID { get; set; }
        public string ProviderName { get; set; }

        public DateTime Date { get; set; }
        public DateTime TimeIn { get; set; }
        public DateTime TimeOut { get; set; }

        public int? ServiceID { get; set; }
        public string ServiceCode { get; set; }

        public int ReportedByID { get; set; }
        public string ReportedByName { get; set; }
        public DateTime ReportedDate { get; set; }

        public string ReportedMessage { get; set; }


    }
}