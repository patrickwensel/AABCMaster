using AABC.Domain2.Notes;
using AABC.DomainServices.Notes;

namespace AABC.Web.App.Notes
{
    public class CaseNotesController : BaseNotesController<CaseNotesService, CaseNoteTasksService, CaseNote, CaseNoteTask>
    {
        public CaseNotesController() : base(SourceType.Case, new CaseNotesService(AppService.Current.DataContextV2),new CaseNoteTasksService(AppService.Current.DataContextV2))
        {
        }
    }
}