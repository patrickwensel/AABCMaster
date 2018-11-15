namespace AABC.Data.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class ProviderServiceZipCode
    {
        public int ID { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime DateCreated { get; set; }

        [Column(TypeName = "timestamp")]
        [MaxLength(8)]
        [Timestamp]
        public byte[] rv { get; set; }

        public int ProviderID { get; set; }

        [Required]
        [StringLength(5)]
        public string ZipCode { get; set; }

        public bool IsPrimary { get; set; }

        public virtual Provider Provider { get; set; }

        public virtual ZipCode ZipCode1 { get; set; }
    }
}
