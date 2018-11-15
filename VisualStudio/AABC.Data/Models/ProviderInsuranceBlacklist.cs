namespace AABC.Data.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("ProviderInsuranceBlacklist")]
    public partial class ProviderInsuranceBlacklist
    {
        public int ID { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime DateCreated { get; set; }

        [Column(TypeName = "timestamp")]
        [MaxLength(8)]
        [Timestamp]
        public byte[] rv { get; set; }

        public int ProviderID { get; set; }

        public int InsuranceID { get; set; }

        [StringLength(255)]
        public string BlacklistReason { get; set; }

        public virtual Insurance Insurance { get; set; }

        public virtual Provider Provider { get; set; }
    }
}
