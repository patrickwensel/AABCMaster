using AABC.Scheduling;
using System;

namespace AABC.Web.App.Scheduler.Models
{
    public class AppointmentCreateRequest
    {
        public int CaseId { get; set; }
        public int ProviderId { get; set; }
        public AppointmentType Type { get; set; }
        public DateTime Date { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }




}