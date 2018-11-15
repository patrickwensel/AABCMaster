namespace AABC.Data.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class CaseAuthHour
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public CaseAuthHour()
        {
            CaseAuthHoursNotes = new HashSet<CaseAuthHoursNote>();
        }

        public int ID { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime DateCreated { get; set; }

        [Column(TypeName = "timestamp")]
        [MaxLength(8)]
        [Timestamp]
        public byte[] rv { get; set; }

        public int? CaseAuthID { get; set; }

        public int CaseProviderID { get; set; }

        [Column(TypeName = "date")]
        public DateTime HoursDate { get; set; }

        public TimeSpan HoursTimeIn { get; set; }

        public TimeSpan HoursTimeOut { get; set; }

        public decimal HoursTotal { get; set; }

        public int? HoursServiceID { get; set; }

        public string HoursNotes { get; set; }

        public int? CaseID { get; set; }

        public int HoursStatus { get; set; }

        public decimal? HoursBillable { get; set; }

        public decimal? HoursPayable { get; set; }

        [StringLength(30)]
        public string HoursBillingRef { get; set; }

        [StringLength(30)]
        public string HoursPayableRef { get; set; }

        public bool HoursHasCatalystData { get; set; }

        public bool HoursWatchEnabled { get; set; }

        [StringLength(255)]
        public string HoursWatchNote { get; set; }

        public int? HoursSSGParentID { get; set; }

        public int? HoursCorrelationID { get; set; }

        [StringLength(255)]
        public string HoursInternalNotes { get; set; }

        public bool IsPayrollOrBillingAdjustment { get; set; }

        public int? ServiceLocationID { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CaseAuthHoursNote> CaseAuthHoursNotes { get; set; }
    }
}
