namespace AABC.Web.App.Hours.Models
{
    public class ScrubOverviewItem
    {
        public int ID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int ActiveProviders { get; set; }
        public int ProvidersWithHours { get; set; }
        public int ProvidersFinalized { get; set; }
        public int ProvidersNotFinalized {
            get
            {
                return ProvidersWithHours - ProvidersFinalized;
            }
        }
        public int ScrubbedRecords { get; set; }
        public int UnscrubbedRecords { get; set; }
        public int CommittedRecords { get; set; }
        public int BilledRecords { get; set; }
        public int PaidRecords { get; set; }
        public decimal TotalPayable { get; set; }
        public decimal TotalBillable { get; set; }
        public decimal BCBABillable { get; set; }
        public decimal AideBillable { get; set; }
        public int BCBAPercent { get; set; }
    }
}