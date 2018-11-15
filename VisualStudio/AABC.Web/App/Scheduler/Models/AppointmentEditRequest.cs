using System;

namespace AABC.Web.App.Scheduler.Models
{
    public class AppointmentEditRequest
    {
        public DateTime Date { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }
}