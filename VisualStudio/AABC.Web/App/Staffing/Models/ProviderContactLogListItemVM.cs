using System;

namespace AABC.Web.App.Staffing.Models
{
    public class ProviderContactLogListItemVM
    {
        public DateTime ContactDate { get; set; }
        public DateTime? FollowUpDate { get; set; }
        public string Status { get; set; }
        public string ProviderName { get; set; }
        public string ProviderTypeCode { get; set; }
        public string Notes { get; set; }
    }
}