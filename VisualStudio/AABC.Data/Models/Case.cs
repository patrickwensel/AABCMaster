namespace AABC.Data.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class Case
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Case()
        {
            CaseAuthCodes = new HashSet<CaseAuthCode>();
            CaseMonthlyPeriods = new HashSet<CaseMonthlyPeriod>();
            CaseProviders = new HashSet<CaseProvider>();
            CaseTasks = new HashSet<CaseTask>();
        }

        public int ID { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime DateCreated { get; set; }

        [Column(TypeName = "timestamp")]
        [MaxLength(8)]
        [Timestamp]
        public byte[] rv { get; set; }

        public int PatientID { get; set; }

        public int? CaseGeneratingReferralID { get; set; }

        public int CaseStatus { get; set; }

        [StringLength(1000)]
        public string CaseStatusNotes { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime? CaseStartDate { get; set; }

        public int? CaseAssignedStaffID { get; set; }

        [StringLength(1000)]
        public string CaseRequiredHoursNotes { get; set; }

        [StringLength(1000)]
        public string CaseRequiredServicesNotes { get; set; }

        public bool CaseHasPrescription { get; set; }

        public bool CaseHasAssessment { get; set; }

        public bool CaseHasIntake { get; set; }

        public int CaseStatusReason { get; set; }

        [StringLength(1000)]
        public string CaseDischargeNotes { get; set; }

        public int? DefaultServiceLocationID { get; set; }

        public bool CaseNeedsRestaffing { get; set; }

        [StringLength(255)]
        public string CaseRestaffingReason { get; set; }

        public bool CaseNeedsStaffing { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CaseAuthCode> CaseAuthCodes { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CaseMonthlyPeriod> CaseMonthlyPeriods { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CaseProvider> CaseProviders { get; set; }

        public virtual Patient Patient { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CaseTask> CaseTasks { get; set; }
    }
}
