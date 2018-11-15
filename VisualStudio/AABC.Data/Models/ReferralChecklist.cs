namespace AABC.Data.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("ReferralChecklist")]
    public partial class ReferralChecklist
    {
        public int ID { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime DateCreated { get; set; }

        [Column(TypeName = "timestamp")]
        [MaxLength(8)]
        [Timestamp]
        public byte[] rv { get; set; }

        public int ReferralID { get; set; }

        public int ChecklistItemID { get; set; }

        public bool ItemIsComplete { get; set; }

        public int? ItemCompletedByStaffID { get; set; }

        public virtual ReferralChecklistItem ReferralChecklistItem { get; set; }

        public virtual Referral Referral { get; set; }
    }
}
