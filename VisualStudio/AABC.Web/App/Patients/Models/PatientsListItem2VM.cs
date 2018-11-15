using AABC.Domain2.Cases;
using AABC.DomainServices.Utils;
using System;

namespace AABC.Web.App.Patients.Models
{
    public class PatientsListItem2VM
    {
        public int ID { get; set; }
        public int PatientID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Domain.Cases.CaseStatus Status { get; set; }
        public Domain.Cases.CaseStatusReason StatusReason { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndingAuthDate { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string County { get; set; }
        public string InsuranceCompanyName { get; set; }
        public string Carrier { get; set; }
        public bool HasAssessment { get; set; }
        public bool HasIntake { get; set; }
        public bool HasPrescription { get; set; }
        public bool HasSupervisor { get; set; }
        public bool NeedsStaffing { get; set; }
        public bool NeedsRestaffing { get; set; }
        public string RestaffingReason { get; set; }
        public RestaffReason? RestaffingReasonID { get; set; }
        public int? PrimaryAideID { get; set; }
        public string AideFirstName { get; set; }
        public string AideLastName { get; set; }
        public int? PrimaryBCBAID { get; set; }
        public string BCBAFirstName { get; set; }
        public string BCBALastName { get; set; }
        public int? AssignedStaffID { get; set; }
        public string AssignedStaffFirstName { get; set; }
        public string AssignedStaffLastName { get; set; }

        public string CommonName { get { return GetCommonName(FirstName, LastName); } }
        public string Aide { get { return GetCommonName(AideFirstName, AideLastName); } }
        public string BCBA { get { return GetCommonName(BCBAFirstName, BCBALastName); } }
        public string CaseManager { get { return GetCommonName(AssignedStaffFirstName, AssignedStaffLastName); } }

        public string RestaffingReasonDisplay
        {
            get
            {
                var s = string.Empty;
                if (RestaffingReasonID.HasValue)
                {
                    s = EnumHelper.GetDescription(RestaffingReasonID.Value);
                }
                return s;
            }
        }

        public string InsuranceDisplay
        {
            get
            {
                var display = InsuranceCompanyName;
                if (!string.IsNullOrEmpty(Carrier))
                {
                    display += " (" + Carrier + ")";
                }
                return display;
            }
        }

        public string StatusText
        {
            get
            {
                return Domain.Cases.Case.GetStatusDisplayString(Status, StatusReason);
            }
        }


        private static string GetCommonName(string firstName, string lastName)
        {
            return (firstName + " " + lastName).Trim();
        }

    }
}