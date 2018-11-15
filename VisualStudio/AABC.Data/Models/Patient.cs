namespace AABC.Data.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class Patient
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Patient()
        {
            Cases = new HashSet<Case>();
        }

        public int ID { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime DateCreated { get; set; }

        [Column(TypeName = "timestamp")]
        [MaxLength(8)]
        [Timestamp]
        public byte[] rv { get; set; }

        public int? PatientGeneratingReferralID { get; set; }

        public int? PatientInsuranceID { get; set; }

        [Required]
        [StringLength(64)]
        public string PatientFirstName { get; set; }

        [Required]
        [StringLength(64)]
        public string PatientLastName { get; set; }

        [Column(TypeName = "date")]
        public DateTime? PatientDateOfBirth { get; set; }

        [StringLength(5)]
        public string PatientGender { get; set; }

        [StringLength(64)]
        public string PatientPrimarySpokenLangauge { get; set; }

        [StringLength(64)]
        public string PatientGuardianFirstName { get; set; }

        [StringLength(64)]
        public string PatientGuardianLastName { get; set; }

        [StringLength(64)]
        public string PatientGuardianRelationship { get; set; }

        [StringLength(128)]
        public string PatientEmail { get; set; }

        [StringLength(30)]
        public string PatientPhone { get; set; }

        [StringLength(255)]
        public string PatientAddress1 { get; set; }

        [StringLength(255)]
        public string PatientAddress2 { get; set; }

        [StringLength(255)]
        public string PatientCity { get; set; }

        [StringLength(2)]
        public string PatientState { get; set; }

        [StringLength(20)]
        public string PatientZip { get; set; }

        [StringLength(255)]
        public string PatientInsuranceCompanyName { get; set; }

        [StringLength(64)]
        public string PatientInsuranceMemberID { get; set; }

        [Column(TypeName = "date")]
        public DateTime? PatientInsurancePrimaryCardholderDateOfBirth { get; set; }

        [StringLength(30)]
        public string PatientInsuranceCompanyProviderPhone { get; set; }

        [StringLength(30)]
        public string PatientPhone2 { get; set; }

        public int? PatientPrimarySpokenLanguageID { get; set; }

        public int? PatientGuardianRelationshipID { get; set; }

        [StringLength(64)]
        public string PatientGuardianEmail { get; set; }

        [StringLength(30)]
        public string PatientGuardianCellPhone { get; set; }

        [StringLength(30)]
        public string PatientGuardianHomePhone { get; set; }

        [StringLength(30)]
        public string PatientGuardianWorkPhone { get; set; }

        [StringLength(1000)]
        public string PatientGuardianNotes { get; set; }

        [StringLength(64)]
        public string PatientGuardian2FirstName { get; set; }

        [StringLength(64)]
        public string PatientGuardian2LastName { get; set; }

        public int? PatientGuardian2RelationshipID { get; set; }

        [StringLength(64)]
        public string PatientGuardian2Email { get; set; }

        [StringLength(30)]
        public string PatientGuardian2CellPhone { get; set; }

        [StringLength(30)]
        public string PatientGuardian2HomePhone { get; set; }

        [StringLength(30)]
        public string PatientGuardian2WorkPhone { get; set; }

        [StringLength(1000)]
        public string PatientGuardian2Notes { get; set; }

        [StringLength(64)]
        public string PatientGuardian3FirstName { get; set; }

        [StringLength(64)]
        public string PatientGuardian3LastName { get; set; }

        public int? PatientGuardian3RelationshipID { get; set; }

        [StringLength(64)]
        public string PatientGuardian3Email { get; set; }

        [StringLength(30)]
        public string PatientGuardian3CellPhone { get; set; }

        [StringLength(30)]
        public string PatientGuardian3HomePhone { get; set; }

        [StringLength(30)]
        public string PatientGuardian3WorkPhone { get; set; }

        [StringLength(1000)]
        public string PatientGuardian3Notes { get; set; }
        
        [StringLength(2000)]
        public string PatientNotes { get; set; }

        [StringLength(128)]
        public string PatientPhysicianName { get; set; }

        [StringLength(128)]
        public string PatientPhysicianAddress { get; set; }

        [StringLength(50)]
        public string PatientPhysicianPhone { get; set; }

        [StringLength(50)]
        public string PatientPhysicianFax { get; set; }

        [StringLength(128)]
        public string PatientPhysicianEmail { get; set; }

        [StringLength(128)]
        public string PatientPhysicianContact { get; set; }

        [StringLength(2000)]
        public string PatientPhysicianNotes { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Case> Cases { get; set; }

        public virtual GuardianRelationship GuardianRelationship { get; set; }
    }
}
