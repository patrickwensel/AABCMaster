using System.Collections.Generic;

namespace AABC.PatientPortal.App.Home.Models
{
    public class HomeVM
    {
        public bool HasSignature { get; set; }
        public List<ChildListItem> Children { get; set; }
        public List<MonthlyGroupListItem> MonthlyGroups { get; set; }
        public int ActiveChild { get; set; }

    }
}