using AABC.DomainServices.Utils;
using Newtonsoft.Json;
using System;

namespace AABC.DomainServices.Payments.Models
{
    public class PaymentScheduleDTO
    {
        public int Id { get; set; }
        [JsonConverter(typeof(OnlyDateConverter))]
        public DateTime ScheduledDate { get; set; }
        public decimal Amount { get; set; }
        public int? PaymentChargeId { get; set; }
    }
}