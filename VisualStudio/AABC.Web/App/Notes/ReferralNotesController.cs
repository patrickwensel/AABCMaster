using AABC.Domain2.Notes;
using AABC.DomainServices.Notes;

namespace AABC.Web.App.Notes
{
    public class ReferralNotesController : BaseNotesController<ReferralNotesService, ReferralNoteTasksService, ReferralNote, ReferralNoteTask>
    {
        public ReferralNotesController() : base(SourceType.Referral, new ReferralNotesService(AppService.Current.DataContextV2), new ReferralNoteTasksService(AppService.Current.DataContextV2))
        {
        }
    }
}