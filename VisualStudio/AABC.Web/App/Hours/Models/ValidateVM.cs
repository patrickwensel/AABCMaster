using System;
using System.Collections.Generic;

namespace AABC.Web.App.Hours.Models
{
    public class ValidateVM {

        public List<ValidationItemVM> Items { get; set; }

        public List<AvailableDate> AvailableDates { get; set; }
        public AvailableDate DefaultDate { get
            {
                DateTime n = DateTime.Now;
                n = n.AddMonths(-1);

                return new AvailableDate()
                {
                    Date = new DateTime(n.Year, n.Month, 1, 0, 0, 0)
                };
            }
        }

        public AvailableDate SelectedDate { get; set; }


    

    }
}