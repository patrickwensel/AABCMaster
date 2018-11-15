namespace AABC.Data.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class CaseProviderNote
    {
        public int ID { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime DateCreated { get; set; }

        [Column(TypeName = "timestamp")]
        [MaxLength(8)]
        [Timestamp]
        public byte[] rv { get; set; }

        public int CaseProviderID { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime ProviderNoteDate { get; set; }

        [Required]
        public string ProviderNote { get; set; }

        public virtual CaseProvider CaseProvider { get; set; }
    }
}
