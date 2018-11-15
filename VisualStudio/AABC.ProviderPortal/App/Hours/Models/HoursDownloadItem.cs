using System;

namespace AABC.ProviderPortal.App.Hours.Models
{
    public class HoursDownloadItem
    {
        public string StatusName { get; set; }
        public DateTime HoursDate { get; set; }
        public TimeSpan HoursTimeIn { get; set; }
        public TimeSpan HoursTimeOut { get; set; }
        public decimal HoursTotal { get; set; }
        public string ServiceCode { get; set; }
        public string HoursNotes { get; set; }
        public string ExtendedNotes { get; set; }
    }
}