using Dymeng.Framework.Web.Mvc.Views;
using System.Collections.Generic;

namespace AABC.Web.Models.Cases
{
    public class TimeBillVM
    {
        public int PatientID { get; set; }
        public string PatientName { get; set; }
        public string PatientGender { get; set; }
        public IViewModelBase Base { get; set; }        
        public List<TimeBillGridListItemVM> GridItems { get; set; }
        public DomainServices.Hours.PeriodMatrixByCase HoursMatrix { get; set; }
    }
}