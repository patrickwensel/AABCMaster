namespace AABC.Data.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class WebUserOption
    {
        public int ID { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime DateCreated { get; set; }

        [Column(TypeName = "timestamp")]
        [MaxLength(8)]
        [Timestamp]
        public byte[] rv { get; set; }

        public int WebUserID { get; set; }

        public int? WebOptionID { get; set; }

        public string WebOptionValue { get; set; }

        public bool? isAllowed { get; set; }

        public virtual WebOption WebOption { get; set; }

        public virtual WebUser WebUser { get; set; }

        public virtual WebUser WebUser1 { get; set; }
    }
}
