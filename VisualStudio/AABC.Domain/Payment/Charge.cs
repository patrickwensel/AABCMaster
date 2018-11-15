namespace AABC.Domain.Payment
{
    public class Charge
    {
        public double Amount { get; set; }
        public string Currency { get; set; }
        public int ExpiryMonth { get; set; }
        public int ExpiryYear { get; set; }

    }
}
