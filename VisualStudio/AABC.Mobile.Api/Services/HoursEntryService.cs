using AABC.Shared.Web.App.HoursEntry;

namespace AABC.Mobile.Api.Services
{
    public class HoursEntryService : HoursEntryServiceBase
    {
        public HoursEntryService()
        {
            EntryMode = Domain2.Cases.HoursEntryMode.ProviderEntry;
        }
    }
}