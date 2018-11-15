namespace AABC.Web.Models.Cases
{
    public class NotesVM
    {
        public Dymeng.Framework.Web.Mvc.Views.IViewModelBase Base;
        public int CaseID { get; set; }
        public int PatientID { get; set; }
        public string PatientName { get; set; }
        public string PatientGender { get; set; }
    }
}