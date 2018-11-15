namespace AABC.DomainServices.Payments
{
    public class OperationResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
    }

    public class ChargeOperationResult : OperationResult
    {
        public int? PaymentChargeId { get; set; }
    }

    public class RecurringChargeOperationResult : OperationResult
    {
        public int? PaymentScheduleId { get; set; }
        public int? PaymentChargeId { get; set; }
    }
}
