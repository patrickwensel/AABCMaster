using System;

namespace AABC.Domain2.Hours
{
    public class ReportLogItem
    {
        public int ID { get; set; }
        public int LoginID { get; set; }
        public int HoursID { get; set; }
        public string Message { get; set; }
        public string ResolvedMessage { get; set; }
        public bool IsResolved { get; set; }
        public DateTime DateReported { get; set; }

        public virtual PatientPortal.Login ReportedBy { get; set; } 
        public virtual Hours Hours { get; set; }

    }
}
