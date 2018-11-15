using System;
using System.Collections.Generic;

namespace AABC.Domain.Patients
{

    public class Patient
    {
        public int? ID { get; set; }
        public DateTime? DateCreated { get; set; }
        public int? GeneratingReferralID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public bool? HighRisk { get; set; }
        public Gender? Gender { get; set; }
        public Referrals.Language Language { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string InsuranceCompanyName { get; set; }
        public string InsuranceMemberID { get; set; }
        public DateTime? InsurancePrimaryCardholderDOB { get; set; }
        public string InsuranceProviderPhone { get; set; }
        public int? InsuranceID { get; set; }

        public string GuardianFirstName { get; set; }
        public string GuardianLastName { get; set; }
        public string GuardianRelationship { get; set; }    // deprecated, use GuardianRelationshipObject
        public General.GuardianRelationship GuardianRelationshipObject { get; set; }
        public string GuardianEmail { get; set; }
        public string GuardianCellPhone { get; set; }
        public string GuardianHomePhone { get; set; }
        public string GuardianWorkPhone { get; set; }
        public string GuardianNotes { get; set; }

        public string Guardian2FirstName { get; set; }
        public string Guardian2LastName { get; set; }
        public General.GuardianRelationship Guardian2RelationshipObject { get; set; }
        public string Guardian2Email { get; set; }
        public string Guardian2CellPhone { get; set; }
        public string Guardian2HomePhone { get; set; }
        public string Guardian2WorkPhone { get; set; }
        public string Guardian2Notes { get; set; }

        public string Guardian3FirstName { get; set; }
        public string Guardian3LastName { get; set; }
        public General.GuardianRelationship Guardian3RelationshipObject { get; set; }
        public string Guardian3Email { get; set; }
        public string Guardian3CellPhone { get; set; }
        public string Guardian3HomePhone { get; set; }
        public string Guardian3WorkPhone { get; set; }
        public string Guardian3Notes { get; set; }

        public string PhysicianName { get; set; }
        public string PhysicianAddress { get; set; }
        public string PhysicianPhone { get; set; }
        public string PhysicianFax { get; set; }
        public string PhysicianEmail { get; set; }
        public string PhysicianContact { get; set; }
        public string PhysicianNotes { get; set; }

        public string Notes { get; set; }

        public IEnumerable<DevExpress.Web.UploadedFile> PrescriptionFile { get; set; }
        public string PrescriptionLocation { get; set; }
        public string PrescriptionFileName { get; set; }

        public string CommonName
        {
            get
            {
                string fn = FirstName ?? "";
                string ln = LastName ?? "";
                string rela = "";
                if (fn == "" && ln == "")
                {
                    fn = GuardianFirstName ?? "";
                    ln = GuardianLastName ?? "";
                    rela = GuardianRelationship ?? "";
                }
                string ret = fn + " " + ln;
                ret += rela == "" ? "" : "(" + rela + ")";
                return ret;
            }
        }

        public Patient()
        {
            DateCreated = DateTime.UtcNow;
        }

    }
}
