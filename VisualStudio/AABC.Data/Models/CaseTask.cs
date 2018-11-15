namespace AABC.Data.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class CaseTask
    {
        public int ID { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime DateCreated { get; set; }

        [Column(TypeName = "timestamp")]
        [MaxLength(8)]
        [Timestamp]
        public byte[] rv { get; set; }

        public int CaseID { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime TaskEnteredOn { get; set; }

        [Required]
        [StringLength(255)]
        public string TaskDescription { get; set; }

        public bool TaskComplete { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime? TaskCompletedDate { get; set; }

        public int? TaskCompletedByStaffID { get; set; }

        public virtual Case Case { get; set; }

        public virtual Staff Staff { get; set; }
    }
}
