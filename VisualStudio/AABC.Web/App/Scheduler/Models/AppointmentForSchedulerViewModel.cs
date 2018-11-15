using AABC.Scheduling;
using System;

namespace AABC.Web.App.Scheduler.Models
{
    public class AppointmentForSchedulerViewModel
    {
        public int Id { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }

        public AppointmentForSchedulerViewModel(Appointment appointment)
        {
            Id = appointment.Id;
            Start = appointment.Date + appointment.StartTime;
            End = appointment.Date + appointment.EndTime;
        }
    }

}