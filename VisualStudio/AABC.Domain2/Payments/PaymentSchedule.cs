using System;

namespace AABC.Domain2.Payments
{
    public class PaymentSchedule
    {
        public int Id { get; set; }
        public int PaymentId { get; set; }
        public int ScheduleNumber { get; set; }
        public DateTime ScheduledDate { get; set; }
        public int? PaymentChargeId { get; set;  }

        public virtual Payment Payment { get; set; }
        public virtual PaymentCharge PaymentCharge { get; set; }
    }
}
