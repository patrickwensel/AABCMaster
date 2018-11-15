using System;

namespace AABC.PatientPortal.App.Home.Models
{
    public class MonthlyGroupHoursListItem
    {


        public int ID { get; set; }
        
        public DateTime Date { get; set; }
        public DateTime TimeIn { get; set; }
        public DateTime TimeOut { get; set; }
        public string ProviderFirstName { get; set; }
        public string ProviderLastName { get; set; }
        public int ProviderID { get; set; }
        public int? ServiceID { get; set; }
        public string Service { get; set; }
        public bool IsApproved { get; set; }
        public bool IsReported { get; set; }

    }
}