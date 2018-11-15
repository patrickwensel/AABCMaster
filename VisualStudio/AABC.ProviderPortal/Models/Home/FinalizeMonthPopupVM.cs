using System;
using System.Collections.Generic;

namespace AABC.ProviderPortal.Models.Home
{
    public class FinalizeMonthPopupVM
    {
        
        public List<FinalizedMonthItem> Items { get; set; }

    }


    public class FinalizedMonthItem
    {
        public int? ID { get; set; }
        public bool IsFinalized { get; set; }
        public DateTime FirstDayOfTargetMonth { get; set; }
        public DateTime? DateFinalized { get; set; }
        public string MonthName {
            get
            {
                return System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(FirstDayOfTargetMonth.Month);
            }
        }
        public int Year {
            get
            {
                return FirstDayOfTargetMonth.Year;
            }
        }
    }


}