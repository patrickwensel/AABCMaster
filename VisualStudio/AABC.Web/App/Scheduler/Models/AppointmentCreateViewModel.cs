using System;

namespace AABC.Web.App.Scheduler.Models
{
    public class AppointmentCreateViewModel
    {
        public int CaseId { get; set; }
        public int ProviderId { get; set; }
        public int AppointmentType { get; set; }
        public DateTime Date { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }
}