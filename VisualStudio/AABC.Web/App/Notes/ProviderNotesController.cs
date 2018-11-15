using AABC.Domain2.Notes;
using AABC.DomainServices.Notes;

namespace AABC.Web.App.Notes
{
    public class ProviderNotesController : BaseNotesController<ProviderNotesService, ProviderNoteTasksService, ProviderNote, ProviderNoteTask>
    {
        public ProviderNotesController() : base(SourceType.Provider, new ProviderNotesService(AppService.Current.DataContextV2), new ProviderNoteTasksService(AppService.Current.DataContextV2))
        {
        }
    }
}