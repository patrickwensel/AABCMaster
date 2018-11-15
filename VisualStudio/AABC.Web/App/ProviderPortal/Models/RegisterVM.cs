namespace AABC.Web.Models.ProviderPortal
{

    public class RegisterVM
    {
        
        public int ProviderID { get; set; }
        public string RegisterAction { get; set; }

        public string ProviderNumber { get; set; }
        public string ProviderName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public bool SendEmail { get; set; }

    }
}