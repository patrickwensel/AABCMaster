using System;

namespace AABC.Domain2.Providers
{
    public class PortalUser
    {
        public int ID { get; set; }

        public int AspNetUserID { get; set; }
        public int ProviderID { get; set; }
        public string UserNumber { get; set; }
        public bool HasAppAccess { get; set; }
        public DateTime DateCreated{ get; set; }

        // Provider parent not mapped due to EF config difficulties...
        
    }
}
