using System.Collections.Generic;

namespace AABC.Web.App.Providers.Models
{
    public class ProviderDetailsDTO
    {
        public string Type { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Address2 { get; set; }
        public string ZipCode { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public IEnumerable<string> ServiceAreas { get; set; }
        public IEnumerable<string> ServiceZipCodes { get; set; }
        public IEnumerable<ProviderInsuranceCredentialDTO> InsuranceCredentials { get; set; }
    }
}