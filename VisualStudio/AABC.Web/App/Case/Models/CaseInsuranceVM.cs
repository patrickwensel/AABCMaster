using AABC.Web.App.Case.Models;
using System.Collections.Generic;

namespace AABC.Web.Models.Cases
{
    public class CaseInsuranceVM
    {
        public Dymeng.Framework.Web.Mvc.Views.IViewModelBase Base;
        public int CaseID { get; set; }
        public string PatientName { get; set; }
        public string PatientGender { get; set; }
        public IEnumerable<CaseInsuranceHistoryDTO> History { get; set; }
    }
}
