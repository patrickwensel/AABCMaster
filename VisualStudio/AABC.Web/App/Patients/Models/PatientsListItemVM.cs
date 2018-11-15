using System;

namespace AABC.Web.Models.Patients
{
    [Obsolete("Use PatientListItem2VM instead")]
    public class PatientsListItemVM
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
        public string Insurance { get; set; }
        public string Carrier { get; set; }
        public bool HasAssessment { get; set; }
        public bool HasIntake { get; set; }
        public bool HasPrescription { get; set; }
        public bool HasSupervisor { get; set; }
        public bool Staffed { get; set; }
        public bool NeedsRestaffing { get; set; }
        public string RestaffingReason { get; set; }
        public string RestaffingReasonDisplay { get; set; }
        public int? RestaffingReasonID { get; set; }

        public Domain.OfficeStaff.OfficeStaff CaseManager { get; set; }
        public Domain.Providers.Provider BCBA { get; set; }
        public Domain.Providers.Provider Aide { get; set; }


        public string InsuranceDisplay
        {
            get
            {
                var display = Insurance;
                if (Carrier != null)
                {
                    display = display + " (" + Carrier + ")";
                }
                return display;
            }
        }



        public string CommonName
        {
            get
            {
                return FirstName + " " + LastName;
            }
        }

        public string StatusText
        {
            get
            {
                return Domain.Cases.Case.GetStatusDisplayString(Status, StatusReason);
            }
        }

    }
}