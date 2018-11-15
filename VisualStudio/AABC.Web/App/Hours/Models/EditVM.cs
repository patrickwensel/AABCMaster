using System;
using System.Collections.Generic;

namespace AABC.Web.App.Hours.Models
{
    public class EditVM
    {

        public enum ViewModes
        {
            FinalizedOnly = 0,
            AllHours = 1
        }

        public List<EditListItem> Items { get; set; }

        public ViewModes ViewMode { get; set; } // 0: Finalized Only, 1: All Hours

        public List<AvailableDate> AvailableDates { get; set; }
        public AvailableDate SelectedDate { get; set; }
        public AvailableDate DefaultDate {
            get
            {
                DateTime n = DateTime.Now;
                n = n.AddMonths(-1);

                return new AvailableDate()
                {
                    Date = new DateTime(n.Year, n.Month, 1, 0, 0, 0)
                    
                };
            }
        }

        public List<Shared.DropDownSource> StatusOptions { get; private set; }

        public EditVM()
        {
            StatusOptions = new List<Shared.DropDownSource>();
            StatusOptions.Add(new Shared.DropDownSource("Pending", 0));
            StatusOptions.Add(new Shared.DropDownSource("Committed", 1));
            StatusOptions.Add(new Shared.DropDownSource("Finalized", 2));
            StatusOptions.Add(new Shared.DropDownSource("Scrubbed", 3));

        }



    }
}