using AABC.Domain2.Referrals;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AABC.Domain2.Patients
{
    public class Patient : BaseReferralInfo
    {
        public int ID { get; set; }
        public DateTime DateCreated { get; set; }

        public int? GeneratingReferralID { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string Gender { get; set; }
        public string PrimarySpokenLangauge { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string InsuranceCompanyName { get; set; }
        public string InsuranceMemberID { get; set; }
        public DateTime? InsurancePrimaryCardholderDateOfBirth { get; set; }
        public string InsuranceCompanyProviderPhone { get; set; }
        public string Phone2 { get; set; }
        public int? PrimarySpokenLanguageID { get; set; }
        public string Notes { get; set; }
        public int? InsuranceID { get; set; }

        public virtual ICollection<Cases.Case> Cases { get; set; }
        public virtual object GeneratingReferral { get; set; }
        public virtual ICollection<PatientPortal.Login> PatientPortalLogins { get; set; }
        public virtual Insurances.Insurance Insurance { get; set; }
        public virtual ICollection<Payments.Payment> Payments { get; set; }



        public Cases.Case ActiveCase
        {
            get
            {
                if (Cases.Count > 0)
                {
                    return Cases.First();
                }
                else
                {
                    return null;
                }
            }
        }

        public string CommonName
        {
            get
            {
                return (FirstName + " " + LastName).Trim();
            }
        }
    }
}
