using System;

namespace AABC.Scheduling
{
    public class Appointment
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        //public AppointmentType Type { get; set; }
    }
}
