namespace AABC.Data.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class WebUserPermission
    {
        public int ID { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime DateCreated { get; set; }

        [Column(TypeName = "timestamp")]
        [MaxLength(8)]
        [Timestamp]
        public byte[] rv { get; set; }

        public int WebUserID { get; set; }

        public int WebPermissionID { get; set; }

        public bool? isAllowed { get; set; }

        public virtual WebPermission WebPermission { get; set; }

        public virtual WebUser WebUser { get; set; }

        public virtual WebUser WebUser1 { get; set; }
    }
}
