namespace AABC.Data.Models.Sprocs
{
    internal class CaseTimeScrubOverview
    {
        public int CaseID { get; set; }
        public string PatientFirstName { get; set; }
        public string PatientLastName { get; set; }
        public int CountOfActiveProviders { get; set; }
        public int CountOfProvidersWithHours { get; set; }
        public int CountOfProvidersFinalized { get; set; }
        public int CountOfScrubbedRecords { get; set; }
        public int CountOfUnscrubbedRecords { get; set; }
        public int CountOfCommittedRecords { get; set; }
        public int CountOfBilledRecords { get; set; }
        public int CountOfPaidRecords { get; set; }
        public decimal TotalPayable { get; set; }
        public decimal TotalBillable { get; set; }
        public decimal BCBABillable { get; set; }
        public decimal AideBillable { get; set; }
        public int BCBAPercent { get; set; }
    }
}
