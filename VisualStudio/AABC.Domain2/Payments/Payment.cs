using AABC.Domain2.PatientPortal;
using System;
using System.Collections.Generic;

namespace AABC.Domain2.Payments
{
    public class Payment
    {
        public int Id { get; set; }
        public PaymentType PaymentType { get; set; }
        public int LoginId { get; set; }
        public int PatientId { get; set; }
        public int? ManagementUserId { get; set; }
        public decimal Amount { get; set; }
        public DateTime? OneTimePaymentDate { get; set; }
        public RecurringFrequency? RecurringFrequency { get; set; }
        public DateTime? RecurringDateStart { get; set; }
        public DateTime? RecurringDateEnd { get; set; }
        public bool Active { get; set; }
        public int? CreditCardId { get; set; }

        public virtual Login Login { get; set; }
        public virtual CreditCard CreditCard { get; set; }
        public virtual Patients.Patient Patient { get; set; }
        public virtual WebUser.WebUser ManagementUser { get; set; }
        public virtual ICollection<PaymentSchedule> PaymentSchedules { get; set; }
        public virtual ICollection<PaymentCharge> PaymentCharges { get; set; }

    }
}
