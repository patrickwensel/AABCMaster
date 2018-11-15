using AABC.Domain2.Cases;

namespace AABC.Web.Models.Providers
{
    public class CaseVM
    {
        public bool Active { get; set; }
        public int CaseID { get; set; }
        public string PatientName { get; set; }
        public CaseStatus Status { get; set; }

    }
}