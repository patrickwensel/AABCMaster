namespace AABC.Data.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class CaseMonthlyPeriodProviderFinalization
    {
        public int ID { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime DateCreated { get; set; }

        [Column(TypeName = "timestamp")]
        [MaxLength(8)]
        [Timestamp]
        public byte[] rv { get; set; }

        public int CaseMonthlyPeriodID { get; set; }

        public int ProviderID { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime DateFinalized { get; set; }

        public virtual CaseMonthlyPeriod CaseMonthlyPeriod { get; set; }

        public virtual Provider Provider { get; set; }
    }
}
