namespace AABC.Data.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class CaseAuthCodeGeneralHour
    {
        public int ID { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime DateCreated { get; set; }

        [Column(TypeName = "timestamp")]
        [MaxLength(8)]
        [Timestamp]
        public byte[] rv { get; set; }

        public int CaseAuthID { get; set; }

        public int HoursYear { get; set; }

        public int HoursMonth { get; set; }

        public decimal HoursApplied { get; set; }

        public virtual CaseAuthCode CaseAuthCode { get; set; }
    }
}
