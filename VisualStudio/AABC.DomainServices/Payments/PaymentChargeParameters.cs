namespace AABC.DomainServices.Payments
{
    public class PaymentChargeParameters
    {
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public int CustomerId { get; set; }
        public string GatewayCardId { get; set; }
        public int CardId { get; set; }
        public int PaymentId { get; set; }
        // This is a shorthand to indicates if the payment was originated from a one-time transaction or a recurring one
        public string ReferenceType { get; set; }
        // what's this for?
        // - If it is a one-time payment, it stores the value of the paymentId (which is already referenced through PaymentId)
        // - if it is a recurring payment, it stores the value of the ScheduledPaymentId
        public int ReferenceId { get; set; }
    }
}
