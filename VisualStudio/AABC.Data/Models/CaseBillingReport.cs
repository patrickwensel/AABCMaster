namespace AABC.Data.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class CaseBillingReport
    {
        public int ID { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime DateCreated { get; set; }

        [Column(TypeName = "timestamp")]
        [MaxLength(8)]
        [Timestamp]
        public byte[] rv { get; set; }

        public int ReportBaseID { get; set; }

        public int ReportPeriodID { get; set; }

        [Required]
        [StringLength(30)]
        public string ReportID { get; set; }

        public int? ReportGeneratedByUserID { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime ReportGenerationDate { get; set; }

        public virtual CaseMonthlyPeriod CaseMonthlyPeriod { get; set; }

        public virtual WebUser WebUser { get; set; }
    }
}
