using System.Collections.Generic;

namespace AABC.Web.App.Hours.Models
{
    public class ScrubOverviewItemSummaryVM
    {

        public string CaseUrl { get; set; }
        public string PatientName { get; set; }
        public List<ScrubOverviewItemSummaryProvider> ActiveProviders { get; set; }
        public List<ScrubOverviewItemSummaryProvider> ProvidersWithHours { get; set; }
        public List<ScrubOverviewItemSummaryProvider> ProvidersWithoutHours { get; set; }
        public List<ScrubOverviewItemSummaryProvider> ProvidersFinalized { get; set; }
        public List<ScrubOverviewItemSummaryProvider> ProvidersNotFinalized { get; set; }

        public ScrubOverviewItemSummaryVM(int caseID) {
            CaseUrl = $"/Case/{caseID}/Manage/TimeAndBilling";
            ActiveProviders = new List<ScrubOverviewItemSummaryProvider>();
            ProvidersWithHours = new List<ScrubOverviewItemSummaryProvider>();
            ProvidersWithoutHours = new List<ScrubOverviewItemSummaryProvider>();
            ProvidersFinalized = new List<ScrubOverviewItemSummaryProvider>();
            ProvidersNotFinalized = new List<ScrubOverviewItemSummaryProvider>();
        }

    }


    public class ScrubOverviewItemSummaryProvider
    {
        public int ID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Url {
            get
            {
                return "/Providers/Edit/" + ID.ToString();
            }
        }
    }

}