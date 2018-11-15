namespace AABC.Data.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class CaseAuthHoursNote
    {
        public int ID { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime DateCreated { get; set; }

        [Column(TypeName = "timestamp")]
        [MaxLength(8)]
        [Timestamp]
        public byte[] rv { get; set; }

        public int HoursID { get; set; }

        public int NotesTemplateID { get; set; }

        [StringLength(2000)]
        public string NotesAnswer { get; set; }

        public virtual CaseAuthHour CaseAuthHour { get; set; }

        public virtual HoursNoteTemplate HoursNoteTemplate { get; set; }
    }
}
