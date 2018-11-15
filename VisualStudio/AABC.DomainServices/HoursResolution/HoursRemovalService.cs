using AABC.Data.V2;
using System.Linq;

namespace AABC.DomainServices.HoursResolution
{
    public class HoursRemovalService
    {

        public static void DeleteHours(int hoursID, CoreContext context)
        {
            var hours = context.Hours.Where(x => x.ID == hoursID || x.SSGParentID == hoursID).ToList();
            context.Hours.RemoveRange(hours);
            context.SaveChanges();
        }


    }
}
