namespace AABC.Data.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class CaseMonthlyPeriod
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public CaseMonthlyPeriod()
        {
            CaseBillingReports = new HashSet<CaseBillingReport>();
            CaseMonthlyPeriodProviderFinalizations = new HashSet<CaseMonthlyPeriodProviderFinalization>();
        }

        public int ID { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime DateCreated { get; set; }

        [Column(TypeName = "timestamp")]
        [MaxLength(8)]
        [Timestamp]
        public byte[] rv { get; set; }

        public int CaseID { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime PeriodFirstDayOfMonth { get; set; }

        [Column(TypeName = "nvarchar")]
        public String WatchComment { get; set; }

        [Column(TypeName = "bit")]
        public bool WatchIgnore { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CaseBillingReport> CaseBillingReports { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CaseMonthlyPeriodProviderFinalization> CaseMonthlyPeriodProviderFinalizations { get; set; }

        public virtual Case Case { get; set; }
    }
}
