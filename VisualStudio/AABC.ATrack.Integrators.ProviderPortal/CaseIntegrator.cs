using AABC.Data.Services;
using AABC.Data.V2;
using AABC.DomainServices.Providers;
using ATrack.Integrators.ProviderPortal.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AABC.ATrack.Integrators.ProviderPortal
{
    public class CaseIntegrator : ICaseIntegrator
    {
        private readonly CoreContext Context;
        private readonly ProviderProvider ProviderProvider;
        private readonly int VisibleAfterEndDateDays;

        public CaseIntegrator(CoreContext context, ProviderProvider providerProvider, int visibleAfterEndDateDays)
        {
            Context = context;
            ProviderProvider = providerProvider;
            VisibleAfterEndDateDays = visibleAfterEndDateDays;
        }
       

        public CaseInfo GetCaseInfoForCaseId(string userId, int caseId)
        {
            var provider = ProviderProvider.GetProvider(Convert.ToInt32(userId));
            if (provider == null)
            {
                throw new InvalidOperationException("Unknown provider");
            }
            CaseInfo caseInfo = null;
            var caseService = new CaseService();
            var selectedCase = caseService.GetActiveCasesByProvider(provider.ID, VisibleAfterEndDateDays).FirstOrDefault(c => c.ID == caseId);
            if (selectedCase != null)
            {
                caseInfo = new CaseInfo
                {
                    CaseId = selectedCase.ID,
                    PatientFirstName = selectedCase.Patient.FirstName,
                    PatientLastName = selectedCase.Patient.LastName,
                    PatientDateOfBirth = selectedCase.Patient.DateOfBirth
                };
            }
            return caseInfo;
        }


        public IEnumerable<CaseInfo> GetCaseInfoForUserId(string userId)
        {
            var provider = ProviderProvider.GetProvider(Convert.ToInt32(userId));
            if (provider == null)
            {
                throw new InvalidOperationException("Unknown provider");
            }
            var caseService = new CaseService();
            var providerCases = caseService.GetActiveCasesByProvider(provider.ID, VisibleAfterEndDateDays);
            var cases = providerCases.Select(c => new CaseInfo
            {
                CaseId = c.ID,
                PatientFirstName = c.Patient.FirstName,
                PatientLastName = c.Patient.LastName,
                PatientDateOfBirth = c.Patient.DateOfBirth
            }).ToList();
            return cases;
        }


        public SessionEmailDetails GetSessionEmailDetails(int sessionId)
        {
            // look up the session
            var hours = Context.Hours.Find(sessionId);
            if (hours == null)
            {
                //log.Error("unknown sessionId " + sessionId.ToString());
                throw new ArgumentException("Invalid SessionId", nameof(sessionId));
            }
            // get the details for the provider and patient
            var c = hours.Case;
            var patient = c.Patient;
            var provider = hours.Provider;
            return new SessionEmailDetails
            {
                ProviderEmailAddress = provider.Email,
                SessionStartTime = hours.Date + hours.StartTime,
                SessionDuration = hours.EndTime - hours.StartTime,
                AbbreviatedPatientName = patient.FirstName + " " + patient.LastName
            };
        }
    }
}
