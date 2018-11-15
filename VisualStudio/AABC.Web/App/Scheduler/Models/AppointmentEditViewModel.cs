using System;

namespace AABC.Web.App.Scheduler.Models
{
    public class AppointmentEditViewModel
    {
        public int AppointmentId { get; set; }
        public bool IsRecurring { get; set; }
        public DateTime Date { get; set; }
        public DateTime OccurrenceDate { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }
}