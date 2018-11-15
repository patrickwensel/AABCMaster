namespace AABC.Data.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class CaseAuthCode
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public CaseAuthCode()
        {
            CaseAuthCodeGeneralHours = new HashSet<CaseAuthCodeGeneralHour>();
        }

        public int ID { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime DateCreated { get; set; }

        [Column(TypeName = "timestamp")]
        [MaxLength(8)]
        [Timestamp]
        public byte[] rv { get; set; }

        public int CaseID { get; set; }

        public int AuthCodeID { get; set; }

        public int AuthClassID { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime AuthStartDate { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime AuthEndDate { get; set; }

        public decimal AuthTotalHoursApproved { get; set; }

        public virtual AuthCode AuthCode { get; set; }

        public virtual CaseAuthClass CaseAuthClass { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CaseAuthCodeGeneralHour> CaseAuthCodeGeneralHours { get; set; }

        public virtual Case Case { get; set; }
    }
}
