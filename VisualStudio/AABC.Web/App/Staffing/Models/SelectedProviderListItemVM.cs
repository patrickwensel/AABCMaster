using System;

namespace AABC.Web.App.Staffing.Models
{
    public class SelectedProviderListItemVM
    {
        public int ProviderID { get; set; }
        public string ProviderFirstName { get; set; }
        public string ProviderLastName { get; set; }
        public string ProviderTypeCode { get; set; }
        public string ProviderCity { get; set; }
        public string ProviderState { get; set; }
        public string ProviderZip { get; set; }
        public string ProviderServiceAreas { get; set; }
        public string ProviderServiceCounties { get; set; }
        public string ProviderLanguages { get; set; }

        public int StaffingLogProviderID { get; set; }
        public bool HasBeenContacted { get; set; }
        public string Status { get; set; }
        public string Notes { get; set; }
        public DateTime? FollowUpDate { get; set; }

        public string ProviderFullName
        {
            get
            {
                return string.Format("{0}, {1}", ProviderLastName, ProviderFirstName);
            }
        }

    }
}