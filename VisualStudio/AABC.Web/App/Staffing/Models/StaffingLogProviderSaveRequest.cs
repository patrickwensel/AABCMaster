using System;

namespace AABC.Web.App.Staffing.Models
{
    public class StaffingLogProviderSaveRequest
    {
        public int StaffingLogProviderID { get; set; }
        public bool HasBeenContacted { get; set; }
        public int? Response { get; set; }
        public string Notes { get; set; }
        public DateTime? FollowUpDate { get; set; }
    }
}