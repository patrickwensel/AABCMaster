using System;

namespace AABC.Domain.Notes
{
    public class Task
    {

        public int ID { get; set; }

        public string Description { get; set; }
        public DateTime DueDate { get; set; }
        
        public bool Completed { get; set; }
        public DateTime? CompletedDate { get; set; }
        public int CompletedByUserID { get; set; }
        public string CompletionNotes { get; set; }

        public int? AssignedToOfficeStaffID { get; set; }
        public virtual OfficeStaff.OfficeStaff AssignedTo { get; set; }

        public int NoteID { get; set; }
        public virtual Note Note { get; set; }

    }
}
