using AABC.Data.V2;
using AABC.Domain2.Hours;
using AABC.Shared.Web.App.HoursEntry.Models.Request;
using Newtonsoft.Json;

namespace AABC.Shared.Web.App.HoursEntry
{
    class Mapper2 : BaseMapper<HoursEntryRequest2VM>
    {

        public Mapper2(CoreContext context) : base(context)
        {
        }

        protected override void MapAide(HoursEntryRequest2VM request, Hours entry)
        {
            if (entry.Report == null)
            {
                entry.Report = new SessionReport();
            }
            entry.Report.Report = JsonConvert.SerializeObject(request.SessionReport);
        }

        protected override void MapBCBA(HoursEntryRequest2VM request, Hours entry)
        {
            MapAide(request, entry);
        }
    }
}
