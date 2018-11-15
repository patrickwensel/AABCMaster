using AABC.DomainServices.HoursResolution;
using AABC.Shared.Web.App.HoursEntry;
using System.Web.Mvc;

namespace AABC.Web.App.HoursEntry
{

    [Authorize]
    public class HoursEntryController : HoursEntryControllerBase<HoursEntryService>
    {

        public HoursEntryController() : base(new HoursEntryService(), EntryApp.Manage) { }

    }
}