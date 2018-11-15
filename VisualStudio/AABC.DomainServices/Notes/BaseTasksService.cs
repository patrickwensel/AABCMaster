using AABC.Data.V2;
using AABC.Domain.Admin;
using AABC.Domain2.Notes;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace AABC.DomainServices.Notes
{
    public abstract class BaseTasksService<TNoteTask> where TNoteTask : BaseNoteTask
    {
        protected SourceType SourceType { get; private set; }
        protected CoreContext Context { get; private set; }

        public BaseTasksService(SourceType sourceType, CoreContext context)
        {
            SourceType = sourceType;
            Context = context;
        }

        protected abstract IDbSet<TNoteTask> GetSet();


        public void Save(NoteTaskDTO t)
        {
            var exists = t.ID > 0;
            var task = exists ? GetSet().Where(x => x.ID == t.ID).FirstOrDefault() : GetSet().Create();
            Mapper.ToNoteTask(t, task);
            if (!exists)
            {
                GetSet().Add(task);
            }
            Context.SaveChanges();
        }


        public void TaskComplete(int taskID, string Remarks)
        {
            var task = GetSet().SingleOrDefault(m => m.ID == taskID);
            task.Completed = true;
            task.CompletedRemarks = Remarks;
            task.CompletedDate = DateTime.Now.Date;
            Context.SaveChanges();
        }


        public NoteTaskDTO GetTask(int taskID)
        {
            var task = GetSet().SingleOrDefault(m => m.ID == taskID);
            return Mapper.ToNoteTaskDTO(task, SourceType);
        }


        public IEnumerable<NoteTaskDTO> GetRecentlyCompletedTaskListForCurrentUser(User user)
        {
            var comparisonDate = DateTime.Now.AddMonths(-3);
            var tasks = GetTasks(user).Where(t => t.Completed == true && t.CompletedDate > comparisonDate);
            return tasks.ToList().Select(x => Mapper.ToNoteTaskDTO(x, SourceType));
        }


        public IEnumerable<NoteTaskDTO> GetIncompleteTaskListForCurrentUser(User user)
        {
            var tasks = GetTasks(user).Where(t => t.Completed == false);
            return tasks.ToList().Select(x => Mapper.ToNoteTaskDTO(x, SourceType));
        }


        public IEnumerable<NoteTaskDTO> GetNoteTasks(int NoteId)
        {
            return GetSet().Where(x => x.NoteID == NoteId).ToList().Select(x => Mapper.ToNoteTaskDTO(x, SourceType));
        }


        public void Remove(int taskID)
        {
            var task = GetSet().SingleOrDefault(m => m.ID == taskID);
            GetSet().Remove(task);
            Context.SaveChanges();
        }


        public string MarkCompleted(int taskID, int userId)
        {
            var task = GetSet().SingleOrDefault(m => m.ID == taskID);
            task.Completed = true;
            task.CompletedByUserID = userId;
            task.CompletedDate = DateTime.Now;
            Context.SaveChanges();
            return task.Description;
        }


        private IQueryable<TNoteTask> GetTasks(User user)
        {
            if (user.StaffMember != null)
            {
                int staffID = user.StaffMember.ID.Value;
                return GetSet().Where(x => x.AssignedToStaffID == staffID || x.AssignedToStaffID == null);
            }
            return GetSet().Where(x => x.AssignedToStaffID == null);
        }
    }
}
