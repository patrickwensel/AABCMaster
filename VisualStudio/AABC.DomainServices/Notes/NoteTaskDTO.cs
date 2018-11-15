using AABC.Domain.OfficeStaff;
using System;
using System.Collections.Generic;

namespace AABC.DomainServices.Notes
{
    public class NoteTaskDTO
    {
        public SourceType SourceType { get; set; }
        public int ID { get; set; }
        public int NoteID { get; set; }
        public int ParentID { get; set; }
        public string Description { get; set; }
        public DateTime? DueDate { get; set; }
        public int? AssignedTo { get; set; }
        public bool Completed { get; set; }
        public int? CompletedBy { get; set; }
        public DateTime? CompletedDate { get; set; }
        public string CompletedRemarks { get; set; }

        #region derivedfields
        public string AssignedToName { get; set; }
        public string AssignedByName { get; set; }
        public string CompletedByName { get; set; }
        public string PatientName { get; set; }
        public string Summary { get; set; }
        #endregion

        public string Action { get; set; }
        public IEnumerable<OfficeStaff> AssignedToList { get; set; }

        public NoteTaskDTO() { }

        public NoteTaskDTO(SourceType sourceType)
        {
            SourceType = sourceType;
        }
    }
}