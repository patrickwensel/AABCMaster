using System;

namespace AABC.Web.App.Hours.Models
{
    public class ValidationItemVM
    {

        public int GridID { get; set; }

        public string Type
        {
            get
            {
                switch (TypeValue)
                {
                    case DomainServices.Hours.CrossHoursValidation.ValidationErrors.CaseOverlap:
                        return "CO";
                    case DomainServices.Hours.CrossHoursValidation.ValidationErrors.ProviderOverlapSelf:
                        return "POS";
                    case DomainServices.Hours.CrossHoursValidation.ValidationErrors.SupervisionMismatch:
                        return "SM";
                    case DomainServices.Hours.CrossHoursValidation.ValidationErrors.AideMaxHoursPerDayPerAide:
                        return "MHD";
                    case DomainServices.Hours.CrossHoursValidation.ValidationErrors.NotesMissing:
                        return "NM";
                    default:
                        return "?";
                }
            }
        }
        public DomainServices.Hours.CrossHoursValidation.ValidationErrors TypeValue { get; set; }

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