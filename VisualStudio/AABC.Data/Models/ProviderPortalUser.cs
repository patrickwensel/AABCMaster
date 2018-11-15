namespace AABC.Data.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class ProviderPortalUser
    {
        public int ID { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime DateCreated { get; set; }

        [Column(TypeName = "timestamp")]
        [MaxLength(8)]
        [Timestamp]
        public byte[] rv { get; set; }

        public int? AspNetUserID { get; set; }

        public int? ProviderID { get; set; }

        public bool ProviderHasAppAccess { get; set; }

        [StringLength(10)]
        public string ProviderUserNumber { get; set; }

        public virtual Provider Provider { get; set; }
    }
}
