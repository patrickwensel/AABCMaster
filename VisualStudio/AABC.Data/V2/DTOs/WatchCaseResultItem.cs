namespace AABC.Data.V2.DTOs
{
    public class WatchCaseResultItem
    {

        public int CaseID { get; set; }
        public int PatientID { get; set; }
        public string PatientFirstName { get; set; }
        public string PatientLastName { get; set; }
        public string ProviderFirstName { get; set; }
        public string ProviderLastName { get; set; }
        public string WatchComment { get; set; }
        public bool? WatchIgnore { get; set; }
            
    }
}
