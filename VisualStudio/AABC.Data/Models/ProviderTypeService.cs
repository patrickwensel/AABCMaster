namespace AABC.Data.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class ProviderTypeService
    {
        public int ID { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime DateCreated { get; set; }

        [Column(TypeName = "timestamp")]
        [MaxLength(8)]
        [Timestamp]
        public byte[] rv { get; set; }

        public int ProviderTypeID { get; set; }

        public int ServiceID { get; set; }

        public int? AssociatedAuthClassID { get; set; }

        public virtual CaseAuthClass CaseAuthClass { get; set; }

        public virtual ProviderType ProviderType { get; set; }

        public virtual Service Service { get; set; }
    }
}
