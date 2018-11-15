namespace AABC.Web.App.Hours.Models
{
    public class UnfinalizedProviderExportItemVM
    {

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public int HoursCount { get; set; }
        public string HasFinalization { get; set; }     // Y or N

    }
}