using AABC.Data.V2;
using AABC.Domain2.Notes;
using System.Data.Entity;
using System.Linq;

namespace AABC.DomainServices.Notes
{
    public class ReferralNotesService : BaseNotesService<ReferralNote, ReferralNoteTask>
    {
        public ReferralNotesService(CoreContext context) : base(SourceType.Referral, context, new ReferralNoteTasksService(context))
        {
        }

        protected override ReferralNote GetNoteByTaskId(int taskId)
        {
            return GetSet().SingleOrDefault(m => m.NoteTasks.Any(t => t.ID == taskId));
        }

        protected override IQueryable<ReferralNote> GetNotesByParentId(int parentId)
        {
            return GetSet().Where(m => m.ReferralID == parentId);
        }

        protected override IDbSet<ReferralNote> GetSet()
        {
            return Context.ReferralNotes;
        }
    }
}