namespace AABC.Data.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class Referral
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Referral()
        {
            ReferralChecklists = new HashSet<ReferralChecklist>();
        }

        public int ID { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime DateCreated { get; set; }

        [Column(TypeName = "timestamp")]
        [MaxLength(8)]
        [Timestamp]
        public byte[] rv { get; set; }

        [Required]
        [StringLength(64)]
        public string ReferralFirstName { get; set; }

        [Required]
        [StringLength(64)]
        public string ReferralLastName { get; set; }

        [Column(TypeName = "date")]
        public DateTime? ReferralDateOfBirth { get; set; }

        [StringLength(5)]
        public string ReferralGender { get; set; }

        [StringLength(64)]
        public string ReferralPrimarySpokenLangauge { get; set; }

        [StringLength(64)]
        public string ReferralGuardianFirstName { get; set; }

        [StringLength(64)]
        public string ReferralGuardianLastName { get; set; }

        [StringLength(64)]
        public string ReferralGuardianRelationship { get; set; }

        [StringLength(128)]
        public string ReferralEmail { get; set; }

        [StringLength(40)]
        public string ReferralPhone { get; set; }

        [StringLength(255)]
        public string ReferralAddress1 { get; set; }

        [StringLength(255)]
        public string ReferralAddress2 { get; set; }

        [StringLength(255)]
        public string ReferralCity { get; set; }

        [StringLength(50)]
        public string ReferralState { get; set; }

        [StringLength(20)]
        public string ReferralZip { get; set; }

        [StringLength(1000)]
        public string ReferralAreaOfConcern { get; set; }

        [StringLength(255)]
        public string ReferralInsuranceCompanyName { get; set; }

        [StringLength(64)]
        public string ReferralInsuranceMemberID { get; set; }

        [Column(TypeName = "date")]
        public DateTime? ReferralInsurancePrimaryCardholderDateOfBirth { get; set; }

        [StringLength(30)]
        public string ReferralInsuranceCompanyProviderPhone { get; set; }

        public int? ReferralSourceType { get; set; }

        [StringLength(255)]
        public string ReferralSourceName { get; set; }

        [StringLength(2000)]
        public string ReferralReferrerNotes { get; set; }

        public int ReferralStatus { get; set; }

        public int? ReferralDismissalReasonID { get; set; }

        [StringLength(128)]
        public string ReferralDismissalReason { get; set; }

        [StringLength(2000)]
        public string ReferralDismissalReasonNotes { get; set; }

        public int? ReferralEnteredByStaffID { get; set; }

        public bool ReferralFollowup { get; set; }

        [Column(TypeName = "date")]
        public DateTime? ReferralFollowupDate { get; set; }

        public int? ReferralAssignedStaffID { get; set; }

        public int? ReferralGeneratedCaseID { get; set; }

        public int? ReferralGeneratedPatientID { get; set; }

        [StringLength(75)]
        public string ReferralServicesRequested { get; set; }

        [StringLength(128)]
        public string ReferralPrimaryCardholderName { get; set; }

        public int? ReferralPrimarySpokenLanguageID { get; set; }

        [StringLength(2000)]
        public string ReferralStatusNotes { get; set; }

        public bool ReferralActive { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ReferralChecklist> ReferralChecklists { get; set; }

        public virtual ReferralDismissalReason ReferralDismissalReason1 { get; set; }

        public virtual ReferralSourceType ReferralSourceType1 { get; set; }

        public virtual Staff Staff { get; set; }
    }
}
