using AABC.Data.V2;
using AABC.Domain2.Notes;
using System.Data.Entity;

namespace AABC.DomainServices.Notes
{
    public class ProviderNoteTasksService : BaseTasksService<ProviderNoteTask>
    {
        public ProviderNoteTasksService(CoreContext context) : base(SourceType.Provider, context)
        {
        }

        protected override IDbSet<ProviderNoteTask> GetSet()
        {
            return Context.ProviderNoteTasks;
        }

    }
}