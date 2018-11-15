namespace AABC.Data.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class ReferralSetting
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ID { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime DateCreated { get; set; }

        [Column(TypeName = "timestamp")]
        [MaxLength(8)]
        [Timestamp]
        public byte[] rv { get; set; }

        [Required]
        [StringLength(64)]
        public string SettingName { get; set; }

        [StringLength(1000)]
        public string SettingDescription { get; set; }

        [StringLength(2000)]
        public string SettingValue { get; set; }

        public int SettingValueType { get; set; }

        public virtual SettingValueType SettingValueType1 { get; set; }
    }
}
