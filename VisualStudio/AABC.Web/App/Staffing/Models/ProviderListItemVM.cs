using AABC.Domain2.Providers;

namespace AABC.Web.App.Staffing.Models
{
    public class ProviderListItemVM
    {
        public int ID { get; set; }
        public string ProviderFirstname { get; set; }
        public string ProviderLastname { get; set; }
        public string ProviderTypeCode { get; set; }
        public string ProviderCity { get; set; }
        public string ProviderState { get; set; }
        public string ProviderZip { get; set; }
        public string ProviderServiceAreas { get; set; }
        public string ProviderServiceCounties { get; set; }
        public string ProviderLanguages { get; set; }
        public string ProviderGender { get; set; }
        public ProviderStatus ProviderStatus { get; set; }
            
        public string ProviderFullName => $"{ProviderLastname}, {ProviderFirstname}";
    }
}