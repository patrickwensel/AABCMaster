using System;

namespace AABC.Domain2.Integrations.Catalyst
{
    public class TimesheetPreloadEntry
    {

        public int ID { get; set; }                 // table PK
        public DateTime ResponseDate { get; set; }  // form response date
        public string PatientName { get; set; }
        public string ProviderName { get; set; }
        public DateTime Date { get; set; }
        public string Notes { get; set; }
        public bool ProviderAgreed { get; set; }
        public bool ParentAgreed { get; set; }
        public int? MappedProviderID { get; set; }
        public int? MappedCaseID { get; set; }
        public bool IsResolved { get; set; }

        public int CaseID { get {
                if (MappedCaseID.HasValue) {
                    return MappedCaseID.Value;
                } else {
                    throw new NullReferenceException("Cannot access CaseID without valid mapping");
                }
            }
        }
        public int ProviderID {
            get {
                if (MappedProviderID.HasValue) {
                    return MappedProviderID.Value;
                } else {
                    throw new NullReferenceException("Cannot access ProviderID without valid mapping");
                }
            }
        }

        
        
    }



}
