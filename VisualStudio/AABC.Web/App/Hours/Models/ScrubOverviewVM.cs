using System;
using System.Collections.Generic;
using System.Globalization;

namespace AABC.Web.App.Hours.Models
{
    public class ScrubOverviewVM
    {

        public List<ScrubOverviewItem> Items { get; set; }

        public List<AvailableDate> AvailableDates { get; set; }
        public AvailableDate SelectedDate { get; set; }


    }

    public class AvailableDate
    {
        public DateTime Date { get; set; }
        public string Display {
            get
            {
                return Date.Year.ToString() + ", " + CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(Date.Month);
            }
        }
    }

}