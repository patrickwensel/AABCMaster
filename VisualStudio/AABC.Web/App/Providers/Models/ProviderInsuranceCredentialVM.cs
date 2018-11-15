using AABC.Data.Models;
using System;
using System.Collections.Generic;
namespace AABC.Web.Models.Shared
{
    public class ProviderInsuranceCredentialVM
    {
        public int Id { get; set; }
        public int ProviderId { get; set; }
        public int? InsuranceId { get; set; }
        public string InsuranceName { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public List<Insurance> InsuranceList  { get; set; }
    }
}