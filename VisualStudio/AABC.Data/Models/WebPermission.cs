namespace AABC.Data.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class WebPermission
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public WebPermission()
        {
            WebUserPermissions = new HashSet<WebUserPermission>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ID { get; set; }

        public int? WebPermissionGroupID { get; set; }

        public string WebPermissionDescription { get; set; }

        [StringLength(50)]
        public string WebPermissionName { get; set; }

        public virtual WebPermissionGroup WebPermissionGroup { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<WebUserPermission> WebUserPermissions { get; set; }
    }
}
