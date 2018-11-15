namespace AABC.PatientPortal.App.Settings.Models
{
    public class SettingsVM
    {

        public int UserID { get; set; }

        public DisplayNameVM DisplayNameVM { get; set; }
        public ChangePasswordVM ChangePasswordVM { get; set; }
        public string Signature { get; set; }

    }
}