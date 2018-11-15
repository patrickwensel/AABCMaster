using System.Collections.Generic;

namespace AABC.DomainServices.Payments.Models
{
    public class PaymentWithScheduledDTO
    {
        public int Id { get; set; }
        public bool Active { get; set; }
        public int PatientId { get; set; }
        public string PatientName { get; set; }
        public string PaymentType { get; set; }
        public string RecurringFrequency { get; set; }
        public IEnumerable<PaymentScheduleDTO> Schedules { get; set; }
    }
}