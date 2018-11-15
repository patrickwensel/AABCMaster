namespace AABC.Domain2.Cases
{
    public class CaseInsurancePayment
    {
        public int Id { get; set; }
        public int CaseInsuranceId { get; set; }
        public decimal Amount { get; set; }
        public System.DateTime PaymentDate { get; set; }
        public string Description { get; set; }

        public virtual CaseInsurance CaseInsurance { get; set; }
    }
}
