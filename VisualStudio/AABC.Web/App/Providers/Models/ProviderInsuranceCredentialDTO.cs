using AABC.Domain2.Providers;
using System;

namespace AABC.Web.App.Providers.Models
{
    public class ProviderInsuranceCredentialDTO
    {
        public int Id { get; set; }
        public string InsuranceName { get; set; }
        public bool IsActive { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public static ProviderInsuranceCredentialDTO Map(ProviderInsuranceCredential obj)
        {
            return new ProviderInsuranceCredentialDTO
            {
                Id = obj.ID,
                InsuranceName = obj.Insurance.Name,
                IsActive = obj.Insurance.Active,
                StartDate = obj.StartDate,
                EndDate = obj.EndDate
            };
        }
    }
}