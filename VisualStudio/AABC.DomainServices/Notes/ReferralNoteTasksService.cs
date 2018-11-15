using AABC.Data.V2;
using AABC.Domain2.Notes;
using System.Data.Entity;

namespace AABC.DomainServices.Notes
{
    public class ReferralNoteTasksService : BaseTasksService<ReferralNoteTask>
    {
        public ReferralNoteTasksService(CoreContext context) : base(SourceType.Referral, context)
        {
        }

        protected override IDbSet<ReferralNoteTask> GetSet()
        {
            return Context.ReferralNoteTasks;
        }

    }
}