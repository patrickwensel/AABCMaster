using AABC.Domain2.Notes;
using System;
using System.Collections.Generic;

namespace AABC.DomainServices.Notes
{
    public class NoteDTO
    {
        public SourceType Source { get; set; }
        public int ID { get; set; }
        public int ParentID { get; set; }
        public bool RequiresFollowup { get; set; }
        public DateTime? FollowupDate { get; set; }
        public CorrespondenceType? CorrespondenceType { get; set; }
        public string CorrespondenceName { get; set; }
        public IEnumerable<NoteTaskDTO> Tasks { get; set; }
        public IEnumerable<string> TaskDescription { get; set; }
        public IEnumerable<DateTime?> TaskDueDate { get; set; }
        public IEnumerable<int?> TaskAssignTo { get; set; }
        public string Comments { get; set; }

        public NoteDTO()
        {
            Tasks = new List<NoteTaskDTO>();
            TaskDescription = new List<string>();
            TaskDueDate = new List<DateTime?>();
            TaskAssignTo = new List<int?>();
        }

        public NoteDTO(SourceType source) : this()
        {
            Source = source;
        }

    }
}
