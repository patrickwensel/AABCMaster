using System;

namespace AABC.DomainServices.Providers
{
    public class ProviderEx
    {
        public int ID { get; set; }
        public DateTime DateCreated { get; set; }

        public bool Active { get; set; }
        public bool IsHired { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string CompanyName { get; set; }
        public string Phone { get; set; }
        public string Phone2 { get; set; }
        public string Fax { get; set; }
        public string Email { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }

        public decimal? HourlyRate { get; set; }
        public string Notes { get; set; }
        public string Availability { get; set; }

        public bool HasBackgroundCheck { get; set; }
        public bool HasReferences { get; set; }
        public bool HasResume { get; set; }
        public bool CanCall { get; set; }
        public bool CanReachByPhone { get; set; }
        public bool CanEmail { get; set; }

        public string DocumentStatus { get; set; }
        public string LBA { get; set; }
        public string NPI { get; set; }
        public string CertificationID { get; set; }
        public string CertificationState { get; set; }
        public DateTime? CertificationRenewalDate { get; set; }
        public DateTime? W9Date { get; set; }
        public string CAQH { get; set; }

        public string ProviderNumber { get; set; }

        public int ProviderTypeID { get; set; }

        public string ProviderTypeName { get; set; }
    }
}
