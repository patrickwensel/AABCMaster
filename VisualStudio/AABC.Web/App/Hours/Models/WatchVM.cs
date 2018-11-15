using System.Collections.Generic;

namespace AABC.Web.App.Hours.Models
{
    public class WatchVM
    {

        public List<AvailableDate> AvailableDates { get; set; }
        public AvailableDate SelectedDate { get; set; }

    }
}