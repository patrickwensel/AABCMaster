using AABC.Domain.Providers;
using System;

namespace AABC.Domain.Cases
{

    public class CaseProvider : Providers.Provider
    {
        public int? CaseProviderID { get; set; }
        //public bool Active { get; set; }
        public bool Supervisor { get; set; }
        public bool Assessor { get; set; }
        public bool InsuranceAuthorizedBCBA { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public void AbsorbProvider(Provider p) {

            Active = p.Active;
            Address1 = p.Address1;
            Address2 = p.Address2;
            Availability = p.Availability;
            CanCall = p.CanCall;
            CanEmail = p.CanEmail;
            CanReachByPhone = p.CanReachByPhone;
            CAQH = p.CAQH;
            CertificationID = p.CertificationID;
            CertificationRenewalDate = p.CertificationRenewalDate;
            CertificationState = p.CertificationState;
            City = p.City;
            CompanyName = p.CompanyName;
            DateCreated = p.DateCreated;
            DocumentStatus = p.DocumentStatus;
            Email = p.Email;
            Fax = p.Fax;
            FirstName = p.FirstName;
            HasBackgroundCheck = p.HasBackgroundCheck;
            HasReferences = p.HasReferences;
            HasResume = p.HasResume;
            HourlyRate = p.HourlyRate;
            ID = p.ID;
            IsHired = p.IsHired;
            Languages = p.Languages;
            LastName = p.LastName;
            LBA = p.LBA;
            Notes = p.Notes;
            NPI = p.NPI;
            Phone = p.Phone;
            Phone2 = p.Phone2;
            PortalUser = p.PortalUser;
            ProviderNumber = p.ProviderNumber;
            ServiceAreas = p.ServiceAreas;
            State = p.State;
            Type = p.Type;
            W9Date = p.W9Date;
            Zip = p.Zip;

        }
    }
}
