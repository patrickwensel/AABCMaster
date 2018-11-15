namespace AABC.Data.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class SMTPAccount
    {
        public int ID { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime DateCreated { get; set; }

        [Column(TypeName = "timestamp")]
        [MaxLength(8)]
        [Timestamp]
        public byte[] rv { get; set; }

        [Required]
        [StringLength(50)]
        public string AccountName { get; set; }

        [StringLength(50)]
        public string AccountDisplayName { get; set; }

        [Required]
        [StringLength(50)]
        public string AccountUsername { get; set; }

        [Required]
        [StringLength(50)]
        public string AccountPassword { get; set; }

        [Required]
        [StringLength(50)]
        public string AccountServer { get; set; }

        public short? AccountPort { get; set; }

        public bool? AccountUseSSL { get; set; }

        public short? AccountAuthMode { get; set; }

        [StringLength(50)]
        public string AccountDefaultFromAddress { get; set; }

        [StringLength(50)]
        public string AccountDefaultReplyAddress { get; set; }
    }
}
