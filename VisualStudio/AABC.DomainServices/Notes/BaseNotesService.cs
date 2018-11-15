using AABC.Data.V2;
using AABC.Domain2.Notes;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace AABC.DomainServices.Notes
{
    public abstract class BaseNotesService<TNote, TNoteTask>
        where TNote : BaseNote
        where TNoteTask : BaseNoteTask
    {
        protected SourceType SourceType { get; private set; }
        protected CoreContext Context { get; private set; }
        private readonly BaseTasksService<TNoteTask> TasksService;

        protected BaseNotesService(
            SourceType sourceType,
            CoreContext context,
            BaseTasksService<TNoteTask> tasksService
            )
        {
            SourceType = SourceType;
            Context = context;
            TasksService = tasksService;
        }

        protected abstract IDbSet<TNote> GetSet();
        protected abstract IQueryable<TNote> GetNotesByParentId(int parentId);
        protected abstract TNote GetNoteByTaskId(int taskId);


        public NoteDTO GetNoteEditNew(int parentId)
        {
            var model = new NoteDTO(SourceType)
            {
                ParentID = parentId
            };
            return model;
        }


        public NoteDTO GetNoteEditExisting(int id)
        {
            var note = GetNoteById(id);
            if (note == null)
            {
                return new NoteDTO(SourceType);
            }
            return Mapper.ToNoteDTO(note, SourceType);
        }


        public NoteDTO GetNoteEditExistingByTaskId(int taskId)
        {
            var note = GetNoteByTaskId(taskId);
            if (note == null)
            {
                return new NoteDTO(SourceType);
            }
            return Mapper.ToNoteDTO(note, SourceType);
        }


        public void SaveNote(NoteDTO model, int userId)
        {
            var exists = model.ID > 0;
            var note = exists ? GetNoteById(model.ID) : GetSet().Create();
            Mapper.ToNote(model, note);
            if (!exists)
            {
                note.EnteredByUserID = userId;
                note.SetParentId(model.ParentID);
                GetSet().Add(note);
            }
            Context.SaveChanges();
            foreach (var task in model.Tasks)
            {
                switch (task.Action.ToLower())
                {
                    case "delete":
                        TasksService.Remove(task.ID);
                        break;
                    default:
                        var taskExists = task.ID > 0;
                        var n = taskExists ? TasksService.GetTask(task.ID) : new NoteTaskDTO(SourceType)
                        {
                            NoteID = note.ID,
                            ParentID = note.GetParentID()
                        };
                        n.Description = task.Description;
                        n.DueDate = task.DueDate;
                        n.AssignedTo = task.AssignedTo;
                        TasksService.Save(n);
                        break;
                }
            }
            Context.SaveChanges();
        }


        public void SaveNoteFollowup(NoteFollowupSaveRequest request, int userId)
        {
            var note = GetNoteById(request.NoteID);
            if (note == null)
            {
                throw new NullReferenceException("Note should not be null");
            }
            note.FollowupComplete = true;
            note.FollowupUserID = userId;
            note.FollowupCompleteDate = DateTime.Now;
            note.FollowupComment = request.FollowupComment;
            Context.SaveChanges();
        }


        public IEnumerable<NoteDetailsDTO> GetNotes(int parentId) // TODO: pagination
        {
            var dataSet = GetNotesByParentId(parentId).OrderByDescending(x => x.ID).ToList();
            var items = dataSet.Select(x => Mapper.ToNoteDetailsDTO(x, SourceType));
            return items;
        }


        public IEnumerable<NoteDetailsDTO> GetNewNotes(int parentId)
        {
            /*todo compare with GetNotes*/
            var note = GetNotesByParentId(parentId).OrderByDescending(x => x.ID).First();
            return new List<NoteDetailsDTO>
            {
                Mapper.ToNoteDetailsDTO(note, SourceType)
            };
        }


        public NoteDetailsDTO GetNoteSummary(int id)
        {
            var note = GetNoteById(id);
            return Mapper.ToNoteDetailsDTO(note, SourceType);
        }


        private TNote GetNoteById(int id)
        {
            return GetSet().SingleOrDefault(m => m.ID == id);
        }

    }
}
