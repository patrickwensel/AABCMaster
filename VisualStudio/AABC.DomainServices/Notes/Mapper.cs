using AABC.Domain2.Notes;
using System.Linq;

namespace AABC.DomainServices.Notes
{
    static class Mapper
    {

        public static NoteDTO ToNoteDTO(BaseNote note, SourceType sourceType)
        {
            return new NoteDTO(sourceType)
            {
                ID = note.ID,
                ParentID = note.GetParentID(),
                CorrespondenceType = note.CorrespondenceType,
                CorrespondenceName = note.CorrespondenceName,
                Comments = note.Comments,
                FollowupDate = note.FollowupDate,
                RequiresFollowup = note.RequiresFollowup,
                Tasks = note.GetTasks()?.Select(m => ToNoteTaskDTO(m, sourceType))
            };
        }


        public static void ToNote(NoteDTO t, BaseNote note)
        {
            note.SetParentId(t.ParentID);
            note.CorrespondenceType = t.CorrespondenceType;
            note.CorrespondenceName = t.CorrespondenceName;
            note.RequiresFollowup = t.RequiresFollowup;
            note.FollowupDate = t.FollowupDate;
            note.Comments = t.Comments;
        }


        public static NoteDetailsDTO ToNoteDetailsDTO(BaseNote note, SourceType sourceType)
        {
            return new NoteDetailsDTO(sourceType)
            {
                ID = note.ID,
                ParentID = note.GetParentID(),
                CorrespondenceType = note.CorrespondenceType,
                CorrespondenceName = note.CorrespondenceName,
                EntryDate = note.EntryDate,
                EnteredByUserID = note.EnteredByUserID,
                Comments = note.Comments,
                FollowupComments = note.FollowupComment,
                IsActive = note.RequiresFollowup && !note.FollowupComplete,
                TaskItems = note.GetTasks()?.Select(m => ToNoteTaskDTO(m, sourceType))
            };
        }


        public static NoteTaskDTO ToNoteTaskDTO(BaseNoteTask task, SourceType sourceType)
        {
            return new NoteTaskDTO(sourceType)
            {
                ID = task.ID,
                NoteID = task.NoteID,
                //r.AssignedByName = task.AssignedToStaff.StaffFirstName + " " + task.AssignedToStaff.StaffLastName;
                AssignedTo = task.AssignedToStaffID,
                AssignedToName = task.AssignedToStaff != null ? task.AssignedToStaff.StaffFirstName + " " + task.AssignedToStaff.StaffLastName : "",
                ParentID = task.GetNote().GetParentID(),
                Completed = task.Completed,
                CompletedBy = task.CompletedByUserID,
                CompletedByName = task.CompletedByUser != null ? task.CompletedByUser.WebUserFirstName + " " + task.CompletedByUser.WebUserLastName : "",
                CompletedDate = task.CompletedDate,
                CompletedRemarks = task.CompletedRemarks,
                Description = task.Description,
                DueDate = task.DueDate,
                PatientName = task.GetNote().GetPatientName(),
                Summary = "(" + (task.DueDate.HasValue ? task.DueDate.Value.ToShortDateString() : "No Due Date") + ") " + (task.AssignedToStaffID.HasValue ? task.AssignedToStaff.StaffFirstName + " " + task.AssignedToStaff.StaffLastName + " " : "") + ": " + task.Description
            };
        }


        public static void ToNoteTask(NoteTaskDTO t, BaseNoteTask task)
        {
            task.AssignedToStaffID = t.AssignedTo;
            task.Completed = t.Completed;
            task.CompletedByUserID = t.CompletedBy;
            task.CompletedDate = t.CompletedDate;
            task.CompletedRemarks = t.CompletedRemarks;
            task.Description = t.Description;
            task.DueDate = t.DueDate;
            task.NoteID = t.NoteID;
        }

    }
}
