namespace AABC.Data.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class ProviderType
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ProviderType()
        {
            Providers = new HashSet<Provider>();
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
        [StringLength(32)]
        public string ProviderTypeCode { get; set; }

        [StringLength(255)]
        public string ProviderTypeName { get; set; }

        public bool ProviderTypeIsOutsourced { get; set; }

        public bool ProviderTypeCanSuperviseCase { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Provider> Providers { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ProviderTypeService> ProviderTypeServices { get; set; }
    }
}
