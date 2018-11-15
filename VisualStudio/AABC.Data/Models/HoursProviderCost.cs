namespace AABC.Data.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("HoursProviderCost")]
    public partial class HoursProviderCost
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ID { get; set; }

        [Key]
        [Column(Order = 1, TypeName = "date")]
        public DateTime HoursDate { get; set; }

        [Key]
        [Column(Order = 2)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CaseID { get; set; }

        [StringLength(255)]
        public string PatientInsuranceCompanyName { get; set; }

        [Key]
        [Column(Order = 3)]
        [StringLength(64)]
        public string PatientFirstName { get; set; }

        [Key]
        [Column(Order = 4)]
        [StringLength(64)]
        public string PatientLastName { get; set; }

        public decimal? ProviderRate { get; set; }

        [Key]
        [Column(Order = 5)]
        public decimal HoursTotal { get; set; }

        public decimal? ProviderCost { get; set; }

        public int? PatientInsuranceID { get; set; }
    }
}
