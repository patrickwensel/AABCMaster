namespace AABC.Data.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class Language
    {
        public int ID { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime DateCreated { get; set; }

        [Column(TypeName = "timestamp")]
        [MaxLength(8)]
        [Timestamp]
        public byte[] rv { get; set; }

        public bool LangIsActive { get; set; }

        [Required]
        [StringLength(3)]
        public string LangBiblioCode { get; set; }

        [StringLength(3)]
        public string LangTermCode { get; set; }

        [StringLength(2)]
        public string LangCommonCode { get; set; }

        [Required]
        [StringLength(255)]
        public string LangEnglishName { get; set; }

        [Required]
        [StringLength(255)]
        public string LangFrenchName { get; set; }
    }
}
