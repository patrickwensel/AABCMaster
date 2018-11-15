using System;
using System.Collections.Generic;

namespace AABC.Web.App.Referrals.Models
{
    public class ReferralDTO
    {
        public int Id { get; set; }
        public DateTime DateCreated { get; set; }
        public Domain.Referrals.ReferralStatus Status { get; set; }
        public string StatusNotes { get; set; }


        // dismissal info
        public int? DismissalReasonId { get; set; }
        public string DismissalReason { get; set; }
        public string DismissalReasonNotes { get; set; }
        public bool FollowUp { get; set; }
        public DateTime? FollowUpDate { get; set; }


        // source info
        public int? SourceTypeId { get; set; }
        public string SourceName { get; set; }
        public string ReferrerNotes { get; set; }
        public int? AssignedStaffId { get; set; }


        //Patient Information
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string PrimaryLanguage { get; set; }
        public string AreaOfConcern { get; set; }
        public string ServicesRequested { get; set; }


        //Guardian Contact Information
        public string GuardianLastName { get; set; }
        public string GuardianFirstName { get; set; }
        public int? GuardianRelationshipId { get; set; }
        public string GuardianEmail { get; set; }
        public string GuardianCellPhone { get; set; }
        public string GuardianHomePhone { get; set; }
        public string GuardianWorkPhone { get; set; }
        public string GuardianNotes { get; set; }

        //Guardian Contact 2 Information
        public string Guardian2FirstName { get; set; }
        public string Guardian2LastName { get; set; }
        public int? Guardian2RelationshipId { get; set; }
        public string Guardian2Email { get; set; }
        public string Guardian2CellPhone { get; set; }
        public string Guardian2HomePhone { get; set; }
        public string Guardian2WorkPhone { get; set; }
        public string Guardian2Notes { get; set; }

        //Guardian Contact 3 Information
        public string Guardian3FirstName { get; set; }
        public string Guardian3LastName { get; set; }
        public int? Guardian3RelationshipId { get; set; }
        public string Guardian3Email { get; set; }
        public string Guardian3CellPhone { get; set; }
        public string Guardian3HomePhone { get; set; }
        public string Guardian3WorkPhone { get; set; }
        public string Guardian3Notes { get; set; }

        //Physician Information
        public string PhysicianName { get; set; }
        public string PhysicianAddress { get; set; }
        public string PhysicianPhone { get; set; }
        public string PhysicianFax { get; set; }
        public string PhysicianEmail { get; set; }
        public string PhysicianContact { get; set; }
        public string PhysicianNotes { get; set; }


        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }


        // Insurance Information
        public string CompanyName { get; set; }
        public string MemberId { get; set; }
        public DateTime? PrimaryCardholderDateOfBirth { get; set; }
        public string PrimaryCardholderName { get; set; }
        public string CompanyProviderPhone { get; set; }
        public string BenefitCheck { get; set; }

        // Statuses
        public int? InsuranceStatus { get; set; }
        public int? IntakeStatus { get; set; }
        public int? RxStatus { get; set; }
        public int? InsuranceCardStatus { get; set; }
        public int? EvaluationStatus { get; set; }
        public int? PolicyBookStatus { get; set; }

        public int? InsuranceCompanyId { get; set; }
        public string FundingType { get; set; }
        public string BenefitType { get; set; }
        public decimal? CoPayAmount { get; set; }
        public decimal? CoInsuranceAmount { get; set; }
        public decimal? DeductibleTotal { get; set; }

        public IEnumerable<ReferralCheckListDTO> Checklist { get; set; }
    }

    public class ReferralCheckListDTO
    {
        public int Id { get; set; }
        public string ChecklistItemDescription { get; set; }
        public bool IsComplete { get; set; }
    }
}