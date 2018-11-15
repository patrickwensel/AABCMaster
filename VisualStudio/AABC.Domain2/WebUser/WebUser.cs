using System;

namespace AABC.Domain2.WebUser
{
    public class WebUser
    {
        public int ID { get; set; }
        public DateTime DateCreated { get; set; }
        public byte[] rv { get; set; }
        public int AspNetUserID { get; set; }
        public int? StaffID { get; set; }
        public string UserName { get; set; }
        public string WebUserFirstName { get; set; }
        public string WebUserLastName { get; set; }
        public string WebUserEmail { get; set; }
        public bool? isActive { get; set; }

        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        //public virtual ICollection<NoteTask> NoteTasks { get; set; }
        public virtual Staff.Staff Staff { get; set; }
    }
}
