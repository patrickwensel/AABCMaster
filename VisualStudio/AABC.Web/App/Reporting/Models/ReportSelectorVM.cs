using AABC.Web.App.Hours.Models;
using AABC.Web.Models.Patients;
using System.Collections.Generic;

namespace AABC.Web.App.Reporting.Models
{
    public class ReportSelectorVM
    {


        public List<AvailableDate> AvailableDates { get; set; }
        public AvailableDate DefaultDate { get; set; }
        public List<InsuranceListItem> AvailableInsurance { get; set; }

    }
}