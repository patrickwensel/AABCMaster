using AABC.Domain2.Providers;
using System;

namespace AABC.Web.Models.Providers
{
    public class ProvidersListItemVM
    {
        public int ID { get; set; }
        public ProviderStatus Status { get; set; }
        public bool Active { get { return Status == ProviderStatus.Active; } }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }

        public string ZipCodes { get; set; }
        public string Counties { get; set; }
        public string Credentials { get; set; }

        public string TypeCode { get; set; }
        public int ActiveCaseCount { get; set; }

        [Obsolete("Used only for legacy code, do not reuse")]
        public ProviderTypeVM Type { get; set; }

        public bool ActiveCaseload => Active && ActiveCaseCount > 0;

        public bool CallForCases => Active && ActiveCaseCount == 0;
    }
}