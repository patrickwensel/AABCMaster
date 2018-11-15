using AABC.Data.V2;
using AABC.Domain2.Referrals;
using System;

namespace AABC.DomainServices.Referrals
{
    public class ReferralAcceptanceService 
    {
        protected CoreContext Context { get; private set; }

        public ReferralAcceptanceService(CoreContext context)
        {
            Context = context;
        }


        public bool HandleAcceptedReferral(Referral referral)
        {
            if (!referral.GeneratedCaseID.HasValue && referral.Status == ReferralStatus.Accepted)
            {
                var patient = ReferralToPatient(referral);
                Context.Patients.Add(patient);
                Context.SaveChanges();

                var @case = ReferralToCase(referral);
                @case.Patient = patient;
                @case.Status = Domain2.Cases.CaseStatus.NotReady;
                @case.StartDate = DateTime.Now;
                @case.StatusReason = Domain2.Cases.CaseStatusReason.NotSet;
                if (referral.InsuranceCompanyID.HasValue)
                {
                    @case.Insurances.Add(new Domain2.Cases.CaseInsurance
                    {
                        InsuranceID = referral.InsuranceCompanyID.Value,
                        FundingType = referral.FundingType,
                        BenefitType = referral.BenefitType,
                        CoPayAmount = referral.CoPayAmount,
                        CoInsuranceAmount = referral.CoInsuranceAmount,
                        DeductibleTotal = referral.DeductibleTotal
                    });
                }
                Context.Cases.Add(@case);
                Context.SaveChanges();

                referral.GeneratedCaseID = @case.ID;
                referral.GeneratedPatientID = patient.ID;
                Context.SaveChanges();
                return true;
            }
            return false;
        }


        private static Domain2.Patients.Patient ReferralToPatient(Referral r)
        {
            var p = new Domain2.Patients.Patient
            {
                Address1 = r.Address1,
                Address2 = r.Address2,
                City = r.City,
                DateCreated = DateTime.UtcNow,
                DateOfBirth = r.DateOfBirth,
                Email = r.Email,
                FirstName = r.FirstName,
                Gender = r.Gender,
                GeneratingReferralID = r.ID,
                GuardianFirstName = r.GuardianFirstName,
                GuardianLastName = r.GuardianLastName,
                GuardianRelationshipID = r.GuardianRelationshipID,
                GuardianEmail = r.GuardianEmail,
                GuardianCellPhone = r.GuardianCellPhone,
                GuardianHomePhone = r.GuardianHomePhone,
                GuardianWorkPhone = r.GuardianWorkPhone,
                GuardianNotes = r.GuardianNotes,
                Guardian2FirstName = r.Guardian2FirstName,
                Guardian2LastName = r.Guardian2LastName,
                Guardian2Email = r.Guardian2Email,
                Guardian2CellPhone = r.Guardian2CellPhone,
                Guardian2HomePhone = r.Guardian2HomePhone,
                Guardian2WorkPhone = r.Guardian2WorkPhone,
                Guardian2RelationshipID = r.Guardian2RelationshipID,
                Guardian2Notes = r.Guardian2Notes,
                Guardian3FirstName = r.Guardian3FirstName,
                Guardian3LastName = r.Guardian3LastName,
                Guardian3RelationshipID = r.Guardian3RelationshipID,
                Guardian3CellPhone = r.Guardian3CellPhone,
                Guardian3HomePhone = r.Guardian3HomePhone,
                Guardian3WorkPhone = r.Guardian3WorkPhone,
                Guardian3Email = r.Guardian3Email,
                Guardian3Notes = r.Guardian3Notes,
                PhysicianName = r.PhysicianName,
                PhysicianAddress = r.PhysicianAddress,
                PhysicianEmail = r.PhysicianEmail,
                PhysicianContact = r.PhysicianContact,
                PhysicianFax = r.PhysicianFax,
                PhysicianNotes = r.PhysicianNotes,
                PhysicianPhone = r.PhysicianPhone,
                InsuranceCompanyName = r.InsuranceCompanyName,
                InsuranceMemberID = r.InsuranceMemberID,
                InsurancePrimaryCardholderDateOfBirth = r.InsurancePrimaryCardholderDOB,
                InsuranceCompanyProviderPhone = r.InsuranceProviderPhone,
                PrimarySpokenLangauge = r.PrimaryLanguage,
                LastName = r.LastName,
                Phone = r.Phone,
                State = r.State,
                Zip = r.ZipCode
            };
            return p;
        }

        private static Domain2.Cases.Case ReferralToCase(Referral r)
        {
            var c = new Domain2.Cases.Case
            {
                DateCreated = DateTime.UtcNow,
                GeneratingReferralID = r.ID,
                StatusNotes = r.StatusNotes,
                RequiredHoursNotes = r.ReferrerNotes
            };
            return c;
        }
    }
}