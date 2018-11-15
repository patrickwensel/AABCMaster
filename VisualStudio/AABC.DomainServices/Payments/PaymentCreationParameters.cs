using AABC.Domain2.Payments;
using AABC.DomainServices.Utils;
using Newtonsoft.Json;
using System;


namespace AABC.DomainServices.Payments
{
    public class PaymentCreationParameters
    {
        public PaymentType PaymentType { get; set; }
        public int PatientId { get; set; }
        public decimal Amount { get; set; }
        [JsonConverter(typeof(OnlyDateConverter))]
        public DateTime? OneTimePaymentDate { get; set; }
        public RecurringFrequency? RecurringFrequency { get; set; }
        [JsonConverter(typeof(OnlyDateConverter))]
        public DateTime? RecurringDateStart { get; set; }
        [JsonConverter(typeof(OnlyDateConverter))]
        public DateTime? RecurringDateEnd { get; set; }
        public string CardHolder { get; set; }
        public string CardNumber { get; set; }
        public string CardExpiryMonth { get; set; }
        public string CardExpiryYear { get; set; }
        public string CardSecurityCode { get; set; }
    }



}
