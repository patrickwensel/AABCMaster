namespace AABC.Web.App.Insurance.Models
{
    public class AuthRuleListItem
    {

        public int ID { get; set; }
        public int ProviderTypeID { get; set; }
        public int ServiceID { get; set; }

        public string ProviderType { get; set; }
        public string Service { get; set; }

        public string InitialStats { get; set; }
        public string FinalStats { get; set; }
        public bool AllowOverlap { get; set; }
        public bool RequiresBCBA { get; set; }
        public bool RequiresPreAuth { get; set; }

        public int InsuranceID { get; set; }
        public string InsuranceName { get; set; }



    }
}