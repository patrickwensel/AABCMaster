using System.Collections.Generic;

namespace AABC.Web.App.Models
{
    public class DashboardYakovVM {

        public List<Hours.Models.AvailableDate> AvailableDates { get; set; }
        public Hours.Models.AvailableDate SelectedDate { get; set; }
        public List<AABC.Web.Services.CommonLists.InsuranceListItem> InsuranceList { get; set; }
        public List<AABC.Web.Services.CommonLists.Insurance2ListItem> Insurance2List { get; set; }

        public List<InsuranceCostListItem> InsuranceCostsByPatient { get; set; }

    }
}