using System;
using System.Collections.Generic;

namespace AABC.Domain2.Notes
{
    public enum CorrespondenceType
    {
        General = 1,
        Parent = 2,
        Provider = 3
    }

    public abstract class BaseNote
    {
        public int ID { get; set; }
        public CorrespondenceType? CorrespondenceType { get; set; }
        public string CorrespondenceName { get; set; }
        public DateTime EntryDate { get; private set; }
        public bool RequiresFollowup { get; set; }
        public DateTime? FollowupDate { get; set; }
        public string Comments { get; set; }
        public bool FollowupComplete { get; set; }
        public DateTime? FollowupCompleteDate { get; set; }
        public string FollowupComment { get; set; }

        public int? FollowupUserID { get; set; }
        public virtual WebUser.WebUser FollowupUser { get; set; }

        public int EnteredByUserID { get; set; }
        public virtual WebUser.WebUser EnteredByUser { get; set; }

        protected BaseNote()
        {
            EntryDate = DateTime.UtcNow;
        }

        public abstract int GetParentID();
        public abstract void SetParentId(int parentId);

        public abstract string GetPatientName();
        public abstract IEnumerable<BaseNoteTask> GetTasks();
    }

    public abstract class BaseNote<TNoteTask> : BaseNote where TNoteTask : BaseNoteTask
    {
        public virtual ICollection<TNoteTask> NoteTasks { get; set; }

        public override IEnumerable<BaseNoteTask> GetTasks()
        {
            return NoteTasks;
        }
    }
}
