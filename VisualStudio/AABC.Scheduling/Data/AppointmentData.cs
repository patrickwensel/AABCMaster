using System;
using System.Collections.Generic;

namespace AABC.Scheduling.Data
{
    public class AppointmentData
    {
        public int Id { get; set; }
        public int CaseId { get; set; }
        public int ProviderId { get; set; }
        public AppointmentType Type { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public virtual AppointmentData RecurringAppointment { get; set; }
        public int? RecurringAppointmentId { get; set; }
        public virtual ICollection<AppointmentData> Cancellations { get; set; }
    }
}
