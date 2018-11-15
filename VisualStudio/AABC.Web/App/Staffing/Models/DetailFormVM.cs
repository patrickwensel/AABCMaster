using System;
using System.Collections.Generic;

namespace AABC.Web.App.Staffing.Models
{
    public class DetailFormVM
    {
        public int ProviderID { get; set; }
        public string ProviderLastName { get; set; }
        public string ProviderFirstName { get; set; }
        public bool Active { get; set; }
        public IEnumerable<ActiveCaseInfo> ActiveCases { get; set; }

        public int StaffingLogProviderID { get; set; }
        public bool HasBeenContacted { get; set; }
        public int? Response { get; set; }
        public string Notes { get; set; }
        public DateTime? FollowUpDate { get; set; }
    }

    public class ActiveCaseInfo
    {
        public int CaseID { get; set; }
        public string PatientLastName { get; set; }
        public string PatientFirstName { get; set; }
    }
}