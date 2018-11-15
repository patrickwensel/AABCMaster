using AABC.DomainServices.Utils;
using Newtonsoft.Json;
using System;

namespace AABC.DomainServices.Payments.Models
{
    public class PaymentChargeDTO
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public string PatientName { get; set; }
        public decimal Amount { get; set; }
        [JsonConverter(typeof(OnlyDateConverter))]
        public DateTime ChargeDate { get; set; }
        public bool IsPatientGenerated { get; set; }
    }
}