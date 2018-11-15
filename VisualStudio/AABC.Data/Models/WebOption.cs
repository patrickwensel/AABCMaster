namespace AABC.Data.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class WebOption
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public WebOption()
        {
            WebUserOptions = new HashSet<WebUserOption>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ID { get; set; }

        public int? WebOptionGroupID { get; set; }

        public string WebOptionDescription { get; set; }

        [StringLength(50)]
        public string WebOptionName { get; set; }

        public virtual WebOptionGroup WebOptionGroup { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<WebUserOption> WebUserOptions { get; set; }
    }
}
