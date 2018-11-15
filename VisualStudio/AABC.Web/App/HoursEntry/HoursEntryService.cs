using AABC.Domain2.Cases;
using AABC.Shared.Web.App.HoursEntry;

namespace AABC.Web.App.HoursEntry
{
    public class HoursEntryService : HoursEntryServiceBase
    {
        public HoursEntryService() : base(AppService.Current.DataContextV2, HoursEntryMode.ManagementEntry) { }
    }
}