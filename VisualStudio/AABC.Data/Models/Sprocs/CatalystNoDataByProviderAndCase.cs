namespace AABC.Data.Models.Sprocs
{
    class CatalystNoDataByProviderAndCase
    {
        
        public int CaseID { get; set; }
        public int ProviderID { get; set; }
        public string ProviderFirstName { get; set; }
        public string ProviderLastName { get; set; }
        public string PatientFirstName { get; set; }
        public string PatientLastName { get; set; }
        public string ProviderPrimaryEmail { get; set; }
        public string ProviderPrimaryPhone { get; set; }
        public string dates { get; set; }

    }
}
