namespace AABC.Domain.Catalyst
{
    public class NoDataByProviderAndCase
    {


        public int CaseID { get; set; }
        public int ProviderID { get; set; }
        public string ProviderFirstName { get; set; }
        public string ProviderLastName { get; set; }
        public string PatientFirstName { get; set; }
        public string PatientLastName { get; set; }
        public string ProviderPhone { get; set; }
        public string ProviderEmail { get; set; }
        public string Dates { get; set; }

    }
}
