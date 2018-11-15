using AABC.Data.V2;
using AABC.Domain2.Notes;
using System.Data.Entity;
using System.Linq;

namespace AABC.DomainServices.Notes
{
    public class CaseNotesService : BaseNotesService<CaseNote, CaseNoteTask>
    {
        public CaseNotesService(CoreContext context) : base(SourceType.Case, context, new CaseNoteTasksService(context))
        {
        }

        protected override CaseNote GetNoteByTaskId(int taskId)
        {
            return GetSet().SingleOrDefault(m => m.NoteTasks.Any(t => t.ID == taskId));
        }

        protected override IQueryable<CaseNote> GetNotesByParentId(int parentId)
        {
            return GetSet().Where(m => m.CaseID == parentId);
        }

        protected override IDbSet<CaseNote> GetSet()
        {
            return Context.CaseNotes;
        }

    }
}