using System;

namespace AABC.Web.App.Hours.Models
{
    public class MHBExportItemVM
    {

        public int HoursID { get; set; }

        public string PatientFN { get; set; }
        public string PatientLN { get; set; }
        public string ProviderFN { get; set; }
        public string ProviderLN { get; set; }
        public string ProviderType { get; set; }

        public int CaseID { get; set; }
        public int PatientID { get; set; }
        public int ProviderID { get; set; }
        public int? SupervisingBCBAID { get; set; }
        public bool IsBCBATimesheet { get; set; }
        public DateTime DateOfService { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public double TotalTime { get; set; }
        public string ServiceCode { get; set; }
        public string PlaceOfService { get; set; }
        public int? PlaceOfServiceID { get; set; }
        public int? InsuranceAuthorizedBCBA { get; set; }

        /// <summary>
        /// Calculates the Insurance Authorized Provider ID for billing reporting
        /// </summary>
        public int? AuthorizedProviderID {
            get
            {


                // Per Kim's request 2016-12-27
                // only return the authorized BCBA ID here
                // or null if none present

                return InsuranceAuthorizedBCBA;

                //if (ProviderType != "BCBA") {
                //    return ProviderID;
                //}

                //if (InsuranceAuthorizedBCBA.HasValue) {
                //    return InsuranceAuthorizedBCBA.Value;
                //} else {
                //    return ProviderID;
                //}

            }
        }


    }
}