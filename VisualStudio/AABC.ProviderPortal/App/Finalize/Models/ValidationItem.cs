using AABC.DomainServices.Hours;
using System;

namespace AABC.ProviderPortal.App.Finalize.Models
{
    public class ValidationItem
    {
        
        public string Type {
            get {
                switch (TypeValue) {
                    case CrossHoursValidation.ValidationErrors.CaseOverlap:
                        return "CO";
                    case CrossHoursValidation.ValidationErrors.ProviderOverlapSelf:
                        return "POS";
                    case CrossHoursValidation.ValidationErrors.SupervisionMismatch:
                        return "SM";
                    case CrossHoursValidation.ValidationErrors.AideMaxHoursPerDayPerAide:
                        return "MHD";
                    case CrossHoursValidation.ValidationErrors.NoCatalystData:
                        return "CAT";
                    case CrossHoursValidation.ValidationErrors.NotesMissing:
                        return "NM";
                    default:
                        return "?";
                }
            }
        }
        public CrossHoursValidation.ValidationErrors TypeValue { get; set; }

        public int SourceHoursID { get; set; }
        public int SourceCaseID { get; set; }
        public string SourcePatientName { get; set; }
        public int SourceProviderID { get; set; }
        public string SourceProviderName { get; set; }
        public string SourceServiceCode { get; set; }
        public DateTime SourceDate { get; set; }
        public DateTime SourceTimeIn { get; set; }
        public DateTime SourceTimeOut { get; set; }

        public int? PartnerHoursID { get; set; }
        public int? PartnerCaseID { get; set; }
        public string PartnerPatientName { get; set; }
        public int? PartnerProviderID { get; set; }
        public string PartnerProviderName { get; set; }
        public string PartnerServiceCode { get; set; }
        public DateTime? PartnerDate { get; set; }
        public DateTime? PartnerTimeIn { get; set; }
        public DateTime? PartnerTimeOut { get; set; }


    }
}