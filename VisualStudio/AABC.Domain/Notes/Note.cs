using System;
using System.Collections.Generic;

namespace AABC.Domain.Notes
{

    public enum CorrespondenceType
    {
        General = 0,
        Parent = 1,
        Provider = 2
    }

    public class Note
    {

        public int ID { get; set; }

        public CorrespondenceType? CorrespondenceType { get; set; }
        public string CorrespondenceName { get; set; }

        // not sure yet if we want these note-level or do something else with them...
        //public bool IsTask { get; set; }
        //public bool IsCompleted { get; set; }

        public DateTime EntryDate { get; set; }

        public int EnteredByUserID { get; set; }
        public Admin.User EnteredBy { get; set; }

        public bool RequiresFollowup { get; set; }
        public DateTime FollowupDate { get; set; }

        public ICollection<Task> Tasks { get; set; }

        public string Comments { get; set; }

        public int CaseID { get; set; }
        public virtual Cases.Case Case { get; set; }

        public DateTime? DateResoved { get; set; }
        public string ResolvedBy { get; set; }
        public int ResolvedUserID { get; set; }
        public string ResolutionNotes { get; set; }


        public bool IsActive {
            get
            {
                if (DateResoved.HasValue) {
                    return false;
                }
                if (Tasks == null || Tasks.Count == 0) {
                    return false;
                }
                foreach (var task in Tasks) {
                    if (!task.Completed) {
                        return true;
                    }
                }

                return false;
            }
        }

    }
}
