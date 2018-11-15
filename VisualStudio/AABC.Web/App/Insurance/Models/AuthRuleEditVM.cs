using System.Collections.Generic;

namespace AABC.Web.App.Insurance.Models
{
    public class AuthRuleEditVM
    {

        public int? ID { get; set; }
        
        public int InsuranceID { get; set; }
        public int ProviderTypeID { get; set; }
        public int ServiceID { get; set; }

        public int? InitialAuthorizationID { get; set; }
        public int? InitialMinimumMinutes { get; set; }
        public int? InitialUnitSize { get; set; }

        public int? FinalAuthorizationID { get; set; }
        public int? FinalMinimumMinutes { get; set; }
        public int? FinalUnitSize { get; set; }

        public bool IsUntimed { get; set; }
        public bool AllowOverlapping { get; set; }
        public bool RequiresAuthorizedBCBA { get; set; }
        public bool RequiresPreAuthorization { get; set; }

        public List<Helpers.CommonListItems.ProviderType> ProviderTypesList { get; set; }
        public List<Helpers.CommonListItems.Service> ServicesList { get; set; }
        public List<Helpers.CommonListItems.AuthCode> AuthCodes { get; set; }

    }
}