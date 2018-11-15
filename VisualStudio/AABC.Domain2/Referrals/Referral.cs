using AABC.Domain2.Notes;
using System;
using System.Collections.Generic;

namespace AABC.Domain2.Referrals
{
    public class Referral : BaseReferralInfo
    {
        public int ID { get; set; }
        public DateTime DateCreated { get; private set; }
        public bool Active { get; set; }
        public ReferralStatus Status { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string StatusNotes { get; set; }
        public string DismissalNote { get; set; }
        public string DismissalNoteExtended { get; set; }
        public bool Followup { get; set; }
        public DateTime? FollowupDate { get; set; }
        public string SourceName { get; set; }
        public string ReferrerNotes { get; set; }
        public string Gender { get; set; }
        public string PrimaryLanguage { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public string AreaOfConcern { get; set; }
        public int? InsuranceCompanyID { get; set; }
        public virtual Insurances.Insurance InsuranceCompany { get; set; }
        public string InsuranceCompanyName { get; set; }
        public string InsuranceMemberID { get; set; }
        public DateTime? InsurancePrimaryCardholderDOB { get; set; }
        public string InsuranceProviderPhone { get; set; }
        public string InsurancePrimaryCardholderName { get; set; }
        public string ServicesRequested { get; set; }
        public int? SourceTypeID { get; set; }
        public ReferralSourceType SourceType { get; set; }
        public int? DismissalReasonID { get; set; }
        public ReferralDismissalReason DismissalReason { get; set; }
        public string BenefitCheck { get; set; }
        public int? EnteredByStaffID { get; set; }
        public virtual Staff.Staff EnteredByStaff { get; set; }
        public int? AssignedStaffID { get; set; }
        public virtual Staff.Staff AssignedStaff { get; set; }
        public int? GeneratedCaseID { get; set; }
        public int? GeneratedPatientID { get; set; }
        public int? InsuranceStatus { get; set; }
        public int? IntakeStatus { get; set; }
        public int? RxStatus { get; set; }
        public int? InsuranceCardStatus { get; set; }
        public int? EvaluationStatus { get; set; }
        public int? PolicyBookStatus { get; set; }
        public string FundingType { get; set; }
        public string BenefitType { get; set; }
        public decimal? CoPayAmount { get; set; }
        public decimal? CoInsuranceAmount { get; set; }
        public decimal? DeductibleTotal { get; set; }

        public virtual ICollection<ReferralNote> Notes { get; set; }
        public virtual ICollection<ReferralChecklist> Checklist { get; set; }

        public Referral()
        {
            DateCreated = DateTime.UtcNow;
            Active = true;
            Notes = new List<ReferralNote>();
            Checklist = new List<ReferralChecklist>();
        }
    }
}
