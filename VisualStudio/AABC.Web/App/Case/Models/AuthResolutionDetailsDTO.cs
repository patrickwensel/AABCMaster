using AABC.Web.Infrastructure;
using Newtonsoft.Json;
using System;

namespace AABC.Web.App.Case.Models
{
    public class AuthResolutionDetailsDTO
    {
        public bool WasResolved { get; set; }
        [JsonConverter(typeof(CustomDateTimeConverter))]
        public DateTime HoursDate { get; set; }
        public decimal BillableHours { get; set; }
        public string ServiceCode { get; set; }
        public string InsuranceName { get; set; }
        public string ProviderTypeCode { get; set; }
        public string AuthMatchRuleDetailJSON { get; set; }
        public string ActiveAuthorizationsJSON { get; set; }
        public string ResolvedAuthCode { get; set; }
        [JsonConverter(typeof(CustomDateTimeConverter))]
        public DateTime? ResolvedCaseAuthStartDate { get; set; }
        [JsonConverter(typeof(CustomDateTimeConverter))]
        public DateTime? ResolvedCaseAuthEndDate { get; set; }
        public int? ResolvedMinutes { get; set; }
    }
}