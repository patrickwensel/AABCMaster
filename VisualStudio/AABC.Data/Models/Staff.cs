namespace AABC.Data.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Staff")]
    public partial class Staff
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Staff()
        {
            CaseTasks = new HashSet<CaseTask>();
            Referrals = new HashSet<Referral>();
            WebUsers = new HashSet<WebUser>();
        }

        public int ID { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime DateCreated { get; set; }

        [Column(TypeName = "timestamp")]
        [MaxLength(8)]
        [Timestamp]
        public byte[] rv { get; set; }

        public bool StaffActive { get; set; }

        [Required]
        [StringLength(64)]
        public string StaffFirstName { get; set; }

        [Required]
        [StringLength(64)]
        public string StaffLastName { get; set; }

        [StringLength(30)]
        public string StaffPrimaryPhone { get; set; }

        [StringLength(128)]
        public string StaffPrimaryEmail { get; set; }

        [Column(TypeName = "date")]
        public DateTime? StaffHireDate { get; set; }

        [Column(TypeName = "date")]
        public DateTime? StaffTerminatedDate { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CaseTask> CaseTasks { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Referral> Referrals { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<WebUser> WebUsers { get; set; }
    }
}
