namespace AABC.Web.App.Hours.Models
{
    public class WatchCaseResultItem
    {


        public int CaseID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string ProviderLastName { get; set; }
        public string ProviderFirstName { get; set; }

        public string Comments { get; set; }
        public bool Ignore { get; set; }

    }
}