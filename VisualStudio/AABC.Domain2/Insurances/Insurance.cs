using System;
using System.Collections.Generic;

namespace AABC.Domain2.Insurances
{
    public class Insurance
    {
        
        public int ID { get; set; }
        public DateTime DateCreated { get; set; }

        public bool Active { get; set; } = true;
        public string Name { get; set; }
        public bool? RequireCredentialsForBCBA { get; set; }

        [Obsolete("Patient level insurance is deprecated, use case insurances instead")]
        public virtual ICollection<Patients.Patient> Patients { get; set; }

        public virtual ICollection<Authorizations.AuthorizationMatchRule> AuthorizationMatchRules { get; set; } = new List<Authorizations.AuthorizationMatchRule>();
        public virtual ICollection<Cases.CaseInsurance> Cases { get; set; } = new List<Cases.CaseInsurance>();
        public virtual ICollection<InsuranceService> Services { get; set; } = new List<InsuranceService>();
        public virtual ICollection<LocalCarrier> LocalCarriers { get; set; } = new List<LocalCarrier>();

    }
}
