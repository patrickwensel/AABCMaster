using AABC.Domain2.Cases;
using AABC.Domain2.Hours;
using AABC.Domain2.Notes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AABC.Domain2.Providers
{
    public class Provider
    {
        public int ID { get; set; }
        public bool IsHired { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string CompanyName { get; set; }
        public string Phone { get; set; }
        public string Phone2 { get; set; }
        public string Fax { get; set; }
        public string Email { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public decimal? HourlyRate { get; set; }
        public string Notes { get; set; }
        public string Availability { get; set; }
        public bool HasBackgroundCheck { get; set; }
        public bool HasReferences { get; set; }
        public bool HasResume { get; set; }
        public bool CanCall { get; set; }
        public bool CanReachByPhone { get; set; }
        public bool CanEmail { get; set; }
        public string DocumentStatus { get; set; }
        public string LBA { get; set; }
        public string NPI { get; set; }
        public string CertificationID { get; set; }
        public string CertificationState { get; set; }
        public DateTime? CertificationRenewalDate { get; set; }
        public DateTime? W9Date { get; set; }
        public string CAQH { get; set; }
        public string ProviderNumber { get; set; }
        public int ProviderTypeID { get; set; }
        public int? ProviderSubTypeID { get; set; }
        public int? PayrollID { get; set; }
        public ProviderStatus Status { get; set; }
        public string ResumeFileName { get; set; }
        public string ResumeLocation { get; set; }
        public string ProviderGender { get; set; }
        public DateTime? HireDate { get; set; }

        // optional PortalUser not mapped due to EF6 config difficulties...

        public virtual ProviderType ProviderType { get; set; }
        public virtual ProviderSubType ProviderSubType { get; set; }
        public virtual ICollection<CaseProvider> Cases { get; set; } = new List<CaseProvider>();
        public virtual ICollection<Hours.Hours> Hours { get; set; } = new List<Hours.Hours>();
        public virtual ICollection<HoursFinalization> Finalizations { get; set; } = new List<HoursFinalization>();
        public virtual ICollection<ProviderRate> ProviderRates { get; set; } = new List<ProviderRate>();
        public virtual ICollection<CaseRate> CaseRates { get; set; } = new List<CaseRate>();
        public virtual ICollection<ServiceRate> ServiceRates { get; set; } = new List<ServiceRate>();
        public virtual ICollection<ProviderServiceZipCode> ServiceZipCodes { get; set; } = new List<ProviderServiceZipCode>();
        public virtual ICollection<ProviderInsuranceCredential> InsuranceCredentials { get; set; } = new List<ProviderInsuranceCredential>();
        public virtual ICollection<ProviderNote> ProviderNotes { get; set; } = new List<ProviderNote>();


        public bool IsBCBA => ProviderTypeID == (int)ProviderTypeIDs.BoardCertifiedBehavioralAnalyst;
        public bool IsAide => ProviderTypeID == (int)ProviderTypeIDs.Aide;


        public IEnumerable<Case> GetActiveCasesAtDate(DateTime refDate)
        {
            var activeCases = Cases
                .Where(x => (!x.StartDate.HasValue || x.StartDate.Value <= refDate)
                    && (!x.EndDate.HasValue || x.EndDate.Value >= refDate)
                    && x.Active
                    && x.Case.Status > CaseStatus.History);
            return activeCases.Select(x => x.Case).ToList();
        }


        public ProviderRate ActiveProviderRate { get { return GetProviderRateAtDate(DateTime.Now); } }


        public ProviderRate GetProviderRateAtDate(DateTime refDate)
        {
            return ProviderRates
                    .Where(x => x.EffectiveDate <= refDate)
                    .OrderByDescending(x => x.EffectiveDate)
                    .FirstOrDefault();
        }


        public CaseRate GetCaseRateAtDate(DateTime refDate, Cases.Case @case)
        {
            return CaseRates
                    .Where(x => x.EffectiveDate <= refDate && x.CaseID == @case.ID)
                    .OrderByDescending(x => x.EffectiveDate)
                    .FirstOrDefault();
        }


        public ServiceRate GetServiceRateAtDate(DateTime refDate, Services.Service service)
        {

            return ServiceRates
                    .Where(x => x.EffectiveDate <= refDate && x.ServiceID == service.ID)
                    .OrderByDescending(x => x.EffectiveDate)
                    .FirstOrDefault();
        }


        public decimal? GetActiveHourlyRate(DateTime refDate, Cases.Case @case, Services.Service service)
        {
            // precedence: Case, Service, Provider, Static
            var caseRate = GetCaseRateAtDate(refDate, @case);
            if (caseRate != null)
            {
                return caseRate.Rate;
            }
            var serviceRate = GetServiceRateAtDate(refDate, service);
            if (serviceRate != null)
            {
                return serviceRate.Rate;
            }
            var providerRate = GetProviderRateAtDate(refDate);
            if (providerRate != null)
            {
                if (providerRate.Type == RateType.Hourly)
                {
                    return providerRate.Rate;
                }
                else
                {
                    throw new NotImplementedException();
                    // need a salary to hours conversion... waiting on client
                }
            }
            return HourlyRate;
        }


        public double GetPendingHoursForCase(int caseID)
        {
            if (Hours == null)
            {
                return 0;
            }
            return (double)Hours.Where(x => x.Status == HoursStatus.Pending && x.CaseID == caseID).Sum(x => x.TotalHours);
        }


        public string GetProviderTypeFullCode()
        {
            return ProviderType.Code + (ProviderSubType != null ? " / " + ProviderSubType.Code : string.Empty);
        }

    }
}
