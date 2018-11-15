namespace AABC.Data.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class ProviderLanguage
    {
        public int ID { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime DateCreated { get; set; }

        [Column(TypeName = "timestamp")]
        [MaxLength(8)]
        [Timestamp]
        public byte[] rv { get; set; }

        public int ProviderID { get; set; }

        public int LanguageID { get; set; }

        public int LanguageStrength { get; set; }

        public virtual ISO639_2_Lang ISO639_2_Lang { get; set; }

        public virtual LanguageStrengthType LanguageStrengthType { get; set; }

        public virtual Provider Provider { get; set; }
    }
}
