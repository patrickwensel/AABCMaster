using System.Collections.Generic;

namespace AABC.ProviderPortal.Models.Home
{
    public class MyCasesVM
    {

        public int ProviderID { get; set; }

        public List<MyCasesCaseItem> Cases { get; set; }
           
        public string IsBCBA { get; set; }              // javascript lowercase true|false

        public bool? SigningSuccess { get; set; }

        // javascript lowercase true|false
        public string UseExtendedBCBAMode {
            get
            {
                if ((Domain.Hours.Note.UseExtendedNotes == true) && (IsBCBA == "true")) {
                    return "true";
                } else {
                    return "false";
                }
            }
        }

        public void FillCases(List<Domain.Cases.Case> cases) {
            foreach (var c in cases) {
                Cases.Add(new MyCasesCaseItem()
                {
                    CaseID = c.ID.Value,
                    PatientName = c.Patient.CommonName
                });
            }
        }

        public MyCasesVM() {
            Cases = new List<MyCasesCaseItem>();
            ProviderID = Global.Default.GetUserProvider().ID.Value;
        }

    }


    public class MyCasesCaseItem
    {
        public int CaseID { get; set; }
        public string PatientName { get; set; }
    }

}