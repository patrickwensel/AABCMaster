using System.Collections.Generic;

namespace AABC.Web.App.Hours.Models
{
    public class WatchDetailPopupVM
    {
        public int CaseID { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }

        public string Comments { get; set; }
        public bool Ignore { get; set; }
        public IEnumerable<Web.Models.Cases.CaseProviderListItem> Providers { get; set; }
    }
}