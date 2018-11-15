using AABC.Data.V2;
using AABC.Domain2.Cases;
using AABC.Shared.Web.App.HoursEntry;

namespace AABC.ATrack.Integrators.ProviderApp
{
    class HoursEntryService : HoursEntryServiceBase
    {
        public HoursEntryService(CoreContext context) : base(context, HoursEntryMode.ProviderEntry)
        {
        }
    }
}
