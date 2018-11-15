using System;
namespace AABC.Domain2.Notes
{
    public abstract class BaseNoteTask
    {
        public int ID { get; set; }
        public int NoteID { get; set; }
        public string Description { get; set; }
        public DateTime? DueDate { get; set; }

        public bool Completed { get; set; }
        public DateTime? CompletedDate { get; set; }
        public string CompletedRemarks { get; set; }

        public int? AssignedToStaffID { get; set; }
        public virtual Staff.Staff AssignedToStaff { get; set; }

        public int? CompletedByUserID { get; set; }
        public virtual WebUser.WebUser CompletedByUser { get; set; }

        public abstract BaseNote GetNote();
    }

    public abstract class BaseNoteTask<TNote> : BaseNoteTask where TNote : BaseNote
    {
        public virtual TNote Note { get; set; }

        public override BaseNote GetNote()
        {
            return Note;
        }
    }
}
