namespace AABC.Data.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class CaseAuthClass
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public CaseAuthClass()
        {
            CaseAuthCodes = new HashSet<CaseAuthCode>();
            ProviderTypeServices = new HashSet<ProviderTypeService>();
        }

        public int ID { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime DateCreated { get; set; }

        [Column(TypeName = "timestamp")]
        [MaxLength(8)]
        [Timestamp]
        public byte[] rv { get; set; }

        [Required]
        [StringLength(12)]
        public string AuthClassCode { get; set; }

        [Required]
        [StringLength(50)]
        public string AuthClassName { get; set; }

        [StringLength(500)]
        public string AuthClassDescription { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CaseAuthCode> CaseAuthCodes { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ProviderTypeService> ProviderTypeServices { get; set; }
    }
}
