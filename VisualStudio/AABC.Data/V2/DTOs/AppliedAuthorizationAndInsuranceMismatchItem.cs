using System;

namespace AABC.Data.V2.DTOs
{
    public class AppliedAuthorizationAndInsuranceMismatchItem
    {
        public long TempKey { get; set; }
        public string PatientFirstName { get; set; }
        public string PatientLastName { get; set; }
        public int? PatientInsuranceID { get; set; }
        public string InsuranceName { get; set; }
        public int CaseID { get; set; }
        public int? CaseAuthCodeID { get; set; }
        public int? AuthCodeID { get; set; }
        public DateTime? AuthStartDate { get; set; }
        public DateTime? AuthEndDate { get; set; }
        public string CodeCode { get; set; }
        public string CodeDescription { get; set; }
    }
}
