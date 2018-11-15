using AABC.Domain2.Cases;
using System;

namespace AABC.Web.App.Hours.Models
{
    public class FinalizationListItem
    {

        public int ID { get; set; }
        public int PatientID { get; set; }
        public int ProviderID { get; set; }

        public DateTime DateFinalized { get; set; }

        public string PatientFirstName { get; set; }
        public string PatientLastName { get; set; }

        public string ProviderFirstName { get; set; }
        public string ProviderLastName { get; set; }

        public bool HasDocuSignDocuments { get; set; } = false;
        public string ViewDocuSignDocuments { get; set; } = "no documents available";

        

        internal static FinalizationListItem MapFromDomain(HoursFinalization f) {

            var item = new FinalizationListItem();

            item.ID = f.ID;
            item.DateFinalized = f.DateFinalized;
            item.PatientID = f.Period.Case.PatientID;
            item.ProviderID = f.ProviderID;
            item.PatientFirstName = f.Period.Case.Patient.FirstName;
            item.PatientLastName = f.Period.Case.Patient.LastName;
            item.ProviderFirstName = f.Provider.FirstName;
            item.ProviderLastName = f.Provider.LastName;

            if (f.EnvelopeID != null && f.IsComplete != 0) {
                item.HasDocuSignDocuments = true;
                item.ViewDocuSignDocuments = "View Documents";
            }

            return item;
        }
    }
}