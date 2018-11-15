using System.Collections;

namespace AABC.Web.App.Scheduler.Models
{
    public class SchedulerDataObject
    {
        public IEnumerable Appointments { get; set; }
        public IEnumerable Resources { get; set; }
    }
}