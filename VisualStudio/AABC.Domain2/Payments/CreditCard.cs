using System.Collections.Generic;

namespace AABC.Domain2.Payments
{
    public class CreditCard
    {
        public int Id { get; set; }
        public int LoginId { get; set; }
        public string CardType { get; set; }
        public string Cardholder { get; set; }
        public string CardNumber { get; set; }
        public string ExpiryMonth { get; set; }
        public string ExpiryYear { get; set; }
        public string GatewayType { get; set; }
        public string GatewayCardId { get; set; }

        public virtual PatientPortal.Login Login { get; set; }
        public virtual ICollection<Payment> Payments { get; set; }
    }
}
