using AABC.Data.V2;
using AABC.Domain2.Notes;
using System.Data.Entity;

namespace AABC.DomainServices.Notes
{
    public class CaseNoteTasksService : BaseTasksService<CaseNoteTask>
    {
        public CaseNoteTasksService(CoreContext context) : base(SourceType.Case, context)
        {
        }

        protected override IDbSet<CaseNoteTask> GetSet()
        {
            return Context.CaseNoteTasks;
        }

    }
}