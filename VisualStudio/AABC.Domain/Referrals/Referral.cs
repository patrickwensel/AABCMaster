using System;
using System.Collections.Generic;

namespace AABC.Domain.Referrals
{

    public enum ReferralStatus
    {
        New = 0,
        InProcess = 1,
        Dismissed = 2,
        Accepted = 3,
        SpecialAttention = 4
    }

    public class Referral
    {
        public int? ID { get; set; }
        public DateTime? DateCreated { get; set; }
        public int? EnteredByStaffID { get; set; }
        public bool Active { get; set; }
        public ReferralStatus Status { get; set; }
        public string StatusNotes { get; set; }
        // dismissal info
        public DismissalReasonType DismissalReasonType { get; set; }
        public string DismissalReason { get; set; }
        public string DismissalReasonNotes { get; set; }
        public bool Followup { get; set; }
        public DateTime? FollowupDate { get; set; }
        // source info
        public SourceType SourceType { get; set; }
        public string SourceName { get; set; }
        public string ReferrerNotes { get; set; }
        // checklist info
        public List<ChecklistItem> ChecklistItems { get; set; }
        // entered by
        // assigned to
        // entered on
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public Gender Gender { get; set; }
        public ContactInfo ContactInfo { get; set; }
        public Language PrimarySpokenLanguage { get; set; }
        public GuardianInfo Guardian { get; set; }
        public string AreaOfConcern { get; set; }
        public string ServicesRequested { get; set; }
        public InsuranceInformation InsuranceInformation { get; set; }
        public OfficeStaff.OfficeStaff AssignedStaff { get; set; }
        public int? GeneratedCaseID { get; set; }
        public int? GeneratedPatientID { get; set; }
        public int? InsuranceStatus { get; set; }
        public int? IntakeStatus { get; set; }
        public int? RxStatus { get; set; }
        public int? InsuranceCardStatus { get; set; }
        public int? EvaluationStatus { get; set; }
        public int? PolicyBookStatus { get; set; }

        public Referral()
        {
            ContactInfo = new ContactInfo();
            Guardian = new GuardianInfo();
            InsuranceInformation = new InsuranceInformation();
            DateCreated = DateTime.UtcNow;
        }

        public Cases.Case GenerateCase()
        {
            var c = new Cases.Case();
            var p = new Patients.Patient
            {
                Address1 = this.ContactInfo.Address1,
                Address2 = this.ContactInfo.Address2,
                City = this.ContactInfo.City,
                DateCreated = DateTime.UtcNow,
                DateOfBirth = this.DateOfBirth,
                Email = this.ContactInfo.Email,
                FirstName = this.FirstName,
                Gender = this.Gender,
                GeneratingReferralID = this.ID,
                GuardianFirstName = this.Guardian.FirstName,
                GuardianLastName = this.Guardian.LastName,
                GuardianRelationship = this.Guardian.Relationship,
                InsuranceCompanyName = this.InsuranceInformation.CompanyName,
                InsuranceMemberID = this.InsuranceInformation.MemberID,
                InsurancePrimaryCardholderDOB = this.InsuranceInformation.PrimaryCardholderDateOfBirth,
                InsuranceProviderPhone = this.InsuranceInformation.CompanyProviderPhone,
                Language = this.PrimarySpokenLanguage,
                LastName = this.LastName,
                Phone = this.ContactInfo.Phone,
                State = this.ContactInfo.State,
                Zip = this.ContactInfo.ZipCode
            };
            c.DateCreated = DateTime.UtcNow;
            c.GeneratingReferralID = this.ID;
            c.Patient = p;
            c.StatusNotes = this.StatusNotes;
            c.RequiredHoursNotes = this.ReferrerNotes;
            c.CalculateStatus();
            return c;
        }
    }


    /***********************

    STATIC TYPED LISTS

    ***********************/
    public class StatusDescriptor
    {
        public int ID { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }

        public static List<StatusDescriptor> GetStatuses()
        {
            List<StatusDescriptor> ret = new List<StatusDescriptor>
            {
                // TODO: switch this to db sourced
                new StatusDescriptor() { ID = 0, Code = "N", Name = "New Referral" },
                new StatusDescriptor() { ID = 1, Code = "IP", Name = "In Process" },
                new StatusDescriptor() { ID = 2, Code = "D", Name = "Dismissed" },
                new StatusDescriptor() { ID = 3, Code = "A", Name = "Accepted" },
                new StatusDescriptor() { ID = 4, Code = "SCA", Name = "Special Attention" }
            };
            return ret;
        }
    }



    /***********************

    DYNAMIC TYPED LISTS

    ***********************/

    public class DismissalReasonType
    {
        public int? ID { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
    }


    public class SourceType
    {
        public int? ID { get; set; }
        public string Name { get; set; }
    }


    public class Language
    {
        public int? ID { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
    }

    public class InsuranceInformation
    {
        public string CompanyName { get; set; }
        public string MemberID { get; set; }
        public DateTime? PrimaryCardholderDateOfBirth { get; set; }
        public string PrimaryCardholderName { get; set; }
        public string CompanyProviderPhone { get; set; }
    }

    public class ContactInfo
    {
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
    }

    public class GuardianInfo
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Relationship { get; set; }
    }



    public class ChecklistItem
    {
        public int? ID { get; set; }
        public int ChecklistItemID { get; set; }
        public string Description { get; set; }
        public DateTime DateCreated { get; set; }
        public bool IsCompleted { get; set; }
        public OfficeStaff.OfficeStaff CompletedBy { get; set; }
    }

}
