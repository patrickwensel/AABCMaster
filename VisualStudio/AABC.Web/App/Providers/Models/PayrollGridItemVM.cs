namespace AABC.Web.Models.Providers
{
    public class PayrollGridItemVM
    {
        public int ID { get; set; }
        public int PayrollID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public double Hours { get; set; }
        public int EntriesMissingCatalystData { get; set; }
    }
}