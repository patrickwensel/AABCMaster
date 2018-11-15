namespace AABC.Data.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class CaseProvider
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public CaseProvider()
        {
            CaseProviderNotes = new HashSet<CaseProviderNote>();
        }

        public int ID { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime DateCreated { get; set; }

        [Column(TypeName = "timestamp")]
        [MaxLength(8)]
        [Timestamp]
        public byte[] rv { get; set; }

        public int CaseID { get; set; }

        public int ProviderID { get; set; }

        public bool Active { get; set; }

        public bool IsSupervisor { get; set; }

        public bool IsAssessor { get; set; }

        public bool IsInsuranceAuthorizedBCBA { get; set; }

        public DateTime? ActiveStartDate { get; set; }

        public DateTime? ActiveEndDate { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CaseProviderNote> CaseProviderNotes { get; set; }

        public virtual Case Case { get; set; }

        public virtual Provider Provider { get; set; }
    }
}
