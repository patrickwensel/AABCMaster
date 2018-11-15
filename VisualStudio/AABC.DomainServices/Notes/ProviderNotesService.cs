using AABC.Data.V2;
using AABC.Domain2.Notes;
using System.Data.Entity;
using System.Linq;

namespace AABC.DomainServices.Notes
{
    public class ProviderNotesService : BaseNotesService<ProviderNote, ProviderNoteTask>
    {
        public ProviderNotesService(CoreContext context) : base(SourceType.Provider, context, new ProviderNoteTasksService(context))
        {
        }

        protected override ProviderNote GetNoteByTaskId(int taskId)
        {
            return GetSet().SingleOrDefault(m => m.NoteTasks.Any(t => t.ID == taskId));
        }

        protected override IQueryable<ProviderNote> GetNotesByParentId(int parentId)
        {
            return GetSet().Where(m => m.ProviderID == parentId);
        }

        protected override IDbSet<ProviderNote> GetSet()
        {
            return Context.ProviderNotes;
        }

    }
}