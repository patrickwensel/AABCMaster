namespace AABC.Data.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class ISO639_2_Lang
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ISO639_2_Lang()
        {
            ProviderLanguages = new HashSet<ProviderLanguage>();
        }

        public int ID { get; set; }

        [StringLength(50)]
        public string ISO_639_2_Bibliographic { get; set; }

        [StringLength(50)]
        public string ISO_639_2_Terminology { get; set; }

        [StringLength(50)]
        public string ISO_639_1 { get; set; }

        [StringLength(50)]
        public string EnglishName { get; set; }

        [StringLength(50)]
        public string FrenchName { get; set; }

        public bool Active { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ProviderLanguage> ProviderLanguages { get; set; }
    }
}
