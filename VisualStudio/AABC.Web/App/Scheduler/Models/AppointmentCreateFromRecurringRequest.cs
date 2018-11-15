using System;

namespace AABC.Web.App.Scheduler.Models
{
    public class AppointmentCreateFromRecurringRequest
    {
        public int RecurringAppointmentId { get; set; }
        public DateTime OccurrenceDate { get; set; }
        public DateTime Date { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }
}