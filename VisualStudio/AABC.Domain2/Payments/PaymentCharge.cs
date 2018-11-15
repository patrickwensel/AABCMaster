using System;

namespace AABC.Domain2.Payments
{
    public class PaymentCharge
    {
        public int Id { get; set; }
        public int LoginId { get; set; }
        public int PaymentId { get; set; }
        public string ReferenceType { get; set; }
        public int ReferenceId { get; set; }
        public int? CreditCardId { get; set; }
        public DateTime ChargeDate { get; set; }
        public string GatewayChargeId { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }
        public string Result { get; set; }
        public string ResultDetails { get; set; }

        public virtual Payment Payment { get; set; }
        public virtual CreditCard CreditCard { get;set;}
    }
}
