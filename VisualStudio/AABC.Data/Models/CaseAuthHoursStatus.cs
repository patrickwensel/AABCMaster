namespace AABC.Data.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("CaseAuthHoursStatuses")]
    public partial class CaseAuthHoursStatus
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ID { get; set; }

        [Required]
        [StringLength(5)]
        public string StatusCode { get; set; }

        [Required]
        [StringLength(10)]
        public string StatusName { get; set; }

        [Required]
        [StringLength(35)]
        public string StatusDescription { get; set; }
    }
}
