using AABC.Domain2.Notes;
using System;
using System.Collections.Generic;

namespace AABC.DomainServices.Notes
{
    public class NoteDetailsDTO
    {
        public SourceType SourceType { get; set; }
        public int ID { get; set; }
        public int ParentID { get; set; }
        public CorrespondenceType? CorrespondenceType { get; set; }
        public string CorrespondenceName { get; set; }
        public DateTime EntryDate { get; set; }
        public int EnteredByUserID { get; set; }
        public string Comments { get; set; }
        public string FollowupComments { get; set; }
        public bool IsActive { get; set; }
        public IEnumerable<NoteTaskDTO> TaskItems { get; set; } = new List<NoteTaskDTO>();

        public NoteDetailsDTO() { }

        public NoteDetailsDTO(SourceType sourceType)
        {
            SourceType = sourceType;
        }


        public string Heading
        {
            get
            {
                var s = EntryDate.ToShortDateString();
                if (IsActive)
                {
                    s += " Active";
                }
                return s;
            }
        }

        public string CorrespondenceSummary
        {
            get
            {
                if (!CorrespondenceType.HasValue)
                {
                    return null;
                }
                else
                {
                    var s = CorrespondenceType.ToString() + " correspondence";
                    if (!string.IsNullOrEmpty(CorrespondenceName))
                    {
                        s += " - " + CorrespondenceName;
                    }
                    return s;
                }
            }
        }
    }
}
