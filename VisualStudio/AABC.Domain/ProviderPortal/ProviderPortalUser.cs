using System;

namespace AABC.Domain.ProviderPortal
{
    public class ProviderPortalUser
    {

        public int? ID { get; set; }
        public DateTime DateCreated { get; set; }

        public int? AspNetUserID { get; set; }
        public Providers.Provider Provider { get; set; }
        public string ProviderUserNumber { get; set; }
        public int? ProviderID { get; set; }
        public bool HasAppAccess { get; set; }

        // (pull from Domain.Providers.Provider
        public string FirstName { get; set; }
        public string LastName { get; set; }
        
    }
}
