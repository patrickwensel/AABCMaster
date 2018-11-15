namespace AABC.Data.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class HoursNoteTemplate
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public HoursNoteTemplate()
        {
            CaseAuthHoursNotes = new HashSet<CaseAuthHoursNote>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ID { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime DateCreated { get; set; }

        [Column(TypeName = "timestamp")]
        [MaxLength(8)]
        [Timestamp]
        public byte[] rv { get; set; }

        public int TemplateGroupID { get; set; }

        public int? TemplateProviderTypeID { get; set; }

        public int? TemplateServiceTypeID { get; set; }

        [Required]
        [StringLength(255)]
        public string TemplateText { get; set; }

        [StringLength(255)]
        public string TemplateTextDescription { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CaseAuthHoursNote> CaseAuthHoursNotes { get; set; }

        public virtual HoursNoteTemplateGroup HoursNoteTemplateGroup { get; set; }
    }
}
