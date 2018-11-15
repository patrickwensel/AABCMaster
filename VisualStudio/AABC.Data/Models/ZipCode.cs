namespace AABC.Data.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class ZipCode
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ZipCode()
        {
            ProviderServiceZipCodes = new HashSet<ProviderServiceZipCode>();
        }

        [Key]
        [Column("ZipCode")]
        [StringLength(5)]
        public string ZipCode1 { get; set; }

        [StringLength(64)]
        public string ZipCity { get; set; }

        [StringLength(2)]
        public string ZipState { get; set; }

        [StringLength(1)]
        public string ZipType { get; set; }

        public int? ZipTimeZone { get; set; }

        public bool? ZipDaylightSavings { get; set; }

        public decimal? ZipLatitude { get; set; }

        public decimal? ZipLongitude { get; set; }

        public bool IsActive { get; set; }

        [StringLength(100)]
        public string ZipCounty { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ProviderServiceZipCode> ProviderServiceZipCodes { get; set; }
    }
}
