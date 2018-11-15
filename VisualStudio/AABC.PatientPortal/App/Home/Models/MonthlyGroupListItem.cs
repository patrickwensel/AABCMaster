using System;
using System.Collections.Generic;

namespace AABC.PatientPortal.App.Home.Models
{
    public class MonthlyGroupListItem
    {

        public string GroupID { get; set; }
        public DateTime MonthDate { get; set; }
        
        public List<MonthlyGroupHoursListItem> Hours { get; set; }

    }
}