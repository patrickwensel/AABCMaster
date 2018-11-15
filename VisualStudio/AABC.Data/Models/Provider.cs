namespace AABC.Data.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class Provider
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Provider()
        {
            CaseMonthlyPeriodProviderFinalizations = new HashSet<CaseMonthlyPeriodProviderFinalization>();
            CaseProviders = new HashSet<CaseProvider>();
            ProviderInsuranceBlacklists = new HashSet<ProviderInsuranceBlacklist>();
            ProviderLanguages = new HashSet<ProviderLanguage>();
            ProviderPortalUsers = new HashSet<ProviderPortalUser>();
            ProviderServiceZipCodes = new HashSet<ProviderServiceZipCode>();
        }

        public int ID { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime DateCreated { get; set; }

        [Column(TypeName = "timestamp")]
        [MaxLength(8)]
        [Timestamp]
        public byte[] rv { get; set; }

        public int? ProviderType { get; set; }

        [Required]
        [StringLength(64)]
        public string ProviderFirstName { get; set; }

        [Required]
        [StringLength(64)]
        public string ProviderLastName { get; set; }

        [StringLength(255)]
        public string ProviderCompanyName { get; set; }

        [StringLength(20)]
        public string ProviderPrimaryPhone { get; set; }

        [StringLength(64)]
        public string ProviderPrimaryEmail { get; set; }

        [StringLength(255)]
        public string ProviderAddress1 { get; set; }

        [StringLength(255)]
        public string ProviderAddress2 { get; set; }

        [StringLength(255)]
        public string ProviderCity { get; set; }

        [StringLength(2)]
        public string ProviderState { get; set; }

        [StringLength(20)]
        public string ProviderZip { get; set; }

        [StringLength(30)]
        public string ProviderNPI { get; set; }

        public decimal? ProviderRate { get; set; }

        [StringLength(30)]
        public string ProviderPhone2 { get; set; }

        [StringLength(30)]
        public string ProviderFax { get; set; }

        [StringLength(1000)]
        public string ProviderNotes { get; set; }

        [StringLength(256)]
        public string ProviderAvailability { get; set; }

        public bool ProviderHasBackgroundCheck { get; set; }

        public bool ProviderHasReferences { get; set; }

        public bool ProviderHasResume { get; set; }

        public bool ProviderCanCall { get; set; }

        public bool ProviderCanReachByPhone { get; set; }

        public bool ProviderCanEmail { get; set; }

        [StringLength(500)]
        public string ProviderDocumentStatus { get; set; }

        [StringLength(50)]
        public string ProviderLBA { get; set; }

        [StringLength(50)]
        public string ProviderCertificationID { get; set; }

        [StringLength(20)]
        public string ProviderCertificationState { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime? ProviderCertificationRenewalDate { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime? ProviderW9Date { get; set; }

        [StringLength(50)]
        public string ProviderCAQH { get; set; }

        [StringLength(30)]
        public string ProviderNumber { get; set; }

        public int ProviderStatus { get; set; }

        public bool ProviderIsHired { get; set; }

        [StringLength(100)]
        public string ResumeFileName { get; set; }

        [StringLength(50)]
        public string ResumeLocation { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CaseMonthlyPeriodProviderFinalization> CaseMonthlyPeriodProviderFinalizations { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CaseProvider> CaseProviders { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ProviderInsuranceBlacklist> ProviderInsuranceBlacklists { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ProviderLanguage> ProviderLanguages { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ProviderPortalUser> ProviderPortalUsers { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ProviderServiceZipCode> ProviderServiceZipCodes { get; set; }

        public virtual ProviderType ProviderType1 { get; set; }
    }
}
