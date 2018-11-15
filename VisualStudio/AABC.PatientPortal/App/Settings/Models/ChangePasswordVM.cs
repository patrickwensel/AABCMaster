namespace AABC.PatientPortal.App.Settings.Models
{
    public class ChangePasswordVM
    {

        public int UserID { get; set; }
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }

    }
}