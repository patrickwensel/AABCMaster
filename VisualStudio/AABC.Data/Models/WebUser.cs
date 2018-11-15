namespace AABC.Data.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class WebUser
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public WebUser()
        {
            CaseBillingReports = new HashSet<CaseBillingReport>();
            WebUserOptions = new HashSet<WebUserOption>();
            WebUserOptions1 = new HashSet<WebUserOption>();
            WebUserPermissions = new HashSet<WebUserPermission>();
            WebUserPermissions1 = new HashSet<WebUserPermission>();
        }

        public int ID { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime DateCreated { get; set; }

        [Column(TypeName = "timestamp")]
        [MaxLength(8)]
        [Timestamp]
        public byte[] rv { get; set; }

        public int AspNetUserID { get; set; }

        public int? StaffID { get; set; }

        [Required]
        [StringLength(128)]
        public string UserName { get; set; }

        [Required]
        [StringLength(64)]
        public string WebUserFirstName { get; set; }

        [Required]
        [StringLength(64)]
        public string WebUserLastName { get; set; }

        [Required]
        [StringLength(128)]
        public string WebUserEmail { get; set; }

        public bool? isActive { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CaseBillingReport> CaseBillingReports { get; set; }

        public virtual Staff Staff { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<WebUserOption> WebUserOptions { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<WebUserOption> WebUserOptions1 { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<WebUserPermission> WebUserPermissions { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<WebUserPermission> WebUserPermissions1 { get; set; }
    }
}
