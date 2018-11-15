using AABC.Domain2.Authorizations;
using AABC.Domain2.Hours;
using AABC.DomainServices.HoursResolution.Logging;
using AABC.DomainServices.HoursResolution.Repositories;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AABC.DomainServices.HoursResolution
{
    class AuthorizationResolution
    {

        private readonly IResolutionServiceRepository ResolutionServiceRepository;
        private readonly HoursResolutionLoggingService LoggingService;

        public AuthorizationResolution(
            IResolutionServiceRepository resolutionServiceRepository,
            HoursResolutionLoggingService hoursResolutionService
        )
        {
            ResolutionServiceRepository = resolutionServiceRepository;
            LoggingService = hoursResolutionService;
        }

        public bool Resolve(IEnumerable<Domain2.Hours.Hours> proposedEntries)
        {

            foreach (var entry in proposedEntries)
            {

                if (entry.AuthorizationBreakdowns == null)
                {
                    entry.AuthorizationBreakdowns = new List<AuthorizationBreakdown>();
                }

                //remove any existing breakdowns
                foreach (var auth in entry.AuthorizationBreakdowns.ToList())
                {
                    ResolutionServiceRepository.RemoveAuthorizationBreakdown(auth);
                    entry.AuthorizationBreakdowns.Remove(auth);
                }
                // map/re-map auth breakdowns
                var auths = getAuthBreakdowns(entry);
                if (auths != null)
                {
                    foreach (var e in auths)
                    {
                        entry.AuthorizationBreakdowns.Add(e);
                    }
                }


            }

            return true;
        }



        private List<AuthorizationBreakdown> getAuthBreakdowns(Domain2.Hours.Hours entry)
        {
            var providerType = entry.Provider.ProviderType;
            var service = entry.Service;
            var insurance = ResolutionServiceRepository.GetActiveInsurance(entry.CaseID, entry.Date);
            if (insurance == null)
            {
                Log(new HoursResolutionLoggingEntry
                {
                    WasResolved = false,
                    HoursID = entry.ID,
                    HoursDate = entry.Date,
                    ServiceID = service.ID,
                    BillableHours = entry.BillableHours,
                    ProviderTypeID = providerType.ID,
                    InsuranceID = 0,
                    AuthMatchRuleDetailJSON = Serialize(new { Message = "No insurance" })
                });
                return null;
            }
            var rules = insurance.AuthorizationMatchRules;
            if (rules == null || rules.Count == 0)
            {
                Log(new HoursResolutionLoggingEntry
                {
                    WasResolved = false,
                    HoursID = entry.ID,
                    HoursDate = entry.Date,
                    ServiceID = service.ID,
                    BillableHours = entry.BillableHours,
                    ProviderTypeID = providerType.ID,
                    InsuranceID = insurance.ID,
                    AuthMatchRuleDetailJSON = Serialize(new { Message = "No rules" })
                });
                return null;
            }
            var matchedRule = rules.Where(x => x.ServiceID == service.ID && x.ProviderTypeID == providerType.ID).FirstOrDefault();
            if (matchedRule == null)
            {
                Log(new HoursResolutionLoggingEntry
                {
                    WasResolved = false,
                    HoursID = entry.ID,
                    HoursDate = entry.Date,
                    ServiceID = service.ID,
                    BillableHours = entry.BillableHours,
                    ProviderTypeID = providerType.ID,
                    InsuranceID = insurance.ID,
                    AuthMatchRuleDetailJSON = Serialize(rules.Select(m => RuleDTO.Create(m)))
                });
                return null;
            }
            var breakdowns = getBreakdownsForRule(entry, matchedRule);
            if (breakdowns == null)
            {
                Log(new HoursResolutionLoggingEntry
                {
                    WasResolved = false,
                    HoursID = entry.ID,
                    HoursDate = entry.Date,
                    ServiceID = service.ID,
                    BillableHours = entry.BillableHours,
                    ProviderTypeID = providerType.ID,
                    InsuranceID = insurance.ID,
                    AuthMatchRuleDetailJSON = Serialize(RuleDTO.Create(matchedRule)),
                    ActiveAuthorizationsJSON = Serialize(entry.Case.GetActiveAuthorizations()?.Select(m => CaseAuthDTO.Create(m)))
                });
            }
            else
            {
                foreach (var b in breakdowns)
                {
                    Log(new HoursResolutionLoggingEntry
                    {
                        WasResolved = true,
                        HoursID = entry.ID,
                        HoursDate = entry.Date,
                        ServiceID = service.ID,
                        BillableHours = entry.BillableHours,
                        ProviderTypeID = providerType.ID,
                        InsuranceID = insurance.ID,
                        AuthMatchRuleDetailJSON = Serialize(RuleDTO.Create(matchedRule)),
                        ResolvedAuthID = b.Authorization?.AuthorizationCode?.ID,
                        ResolvedAuthCode = b.Authorization?.AuthorizationCode?.Code,
                        ResolvedCaseAuthID = b.Authorization?.ID,
                        ResolvedCaseAuthStart = b.Authorization?.StartDate,
                        ResolvedCaseAuthEndDate = b.Authorization?.EndDate,
                        ResolvedAuthMatchRuleID = matchedRule.ID,
                        ResolvedMinutes = b.Minutes
                    });
                }
            }
            return breakdowns;

        }

        private List<AuthorizationBreakdown> getBreakdownsForRule(Domain2.Hours.Hours entry, AuthorizationMatchRule matchedRule)
        {

            var breakdowns = new List<AuthorizationBreakdown>();
            var activeCaseAuths = ResolutionServiceRepository.GetCase(entry.CaseID).GetActiveAuthorizations(entry.Date);
            int totalMinutes = (int)Math.Round(entry.TotalHours * 60);

            if (!matchedRule.IsInitialAuthUsableForTimeEntry)
            {
                return null;
            }

            if (totalMinutes < matchedRule.InitialMinimumMinutes)
            {
                return null;
            }

            var initialAuth = activeCaseAuths.Where(x => x.AuthorizationCodeID == matchedRule.InitialAuthorizationID).FirstOrDefault();

            if (initialAuth == null)
            {
                return null;
            }

            var initialBreakdown = new AuthorizationBreakdown();

            initialBreakdown.AuthorizationID = matchedRule.InitialAuthorization.ID;
            initialBreakdown.Authorization = initialAuth;
            initialBreakdown.HoursEntry = entry;
            //initialBreakdown.HoursID = _hours.ID;
            initialBreakdown.Minutes = matchedRule.GetApplicableInitialMinutes(totalMinutes);

            breakdowns.Add(initialBreakdown);

            if (!matchedRule.IsFinalAuthUsableForTimeEntry)
            {
                return breakdowns;
            }

            var finalAuth = activeCaseAuths.Where(x => x.AuthorizationCodeID == matchedRule.FinalAuthorizationID).FirstOrDefault();

            if (finalAuth == null)
            {
                return breakdowns;
            }

            var finalBreakdown = new AuthorizationBreakdown();
            finalBreakdown.AuthorizationID = matchedRule.FinalAuthorization.ID;
            finalBreakdown.Authorization = finalAuth;
            finalBreakdown.HoursEntry = entry;
            //finalBreakdown.HoursID = entry.ID;
            finalBreakdown.Minutes = matchedRule.GetApplicableFinalMinutes(totalMinutes);

            if (finalBreakdown.Minutes > 0)
            {
                breakdowns.Add(finalBreakdown);
            }

            return breakdowns;

        }


        private void Log(HoursResolutionLoggingEntry e)
        {
            if (LoggingService != null && e != null)
            {
                LoggingService.Log(e);
            }
        }

        private static string Serialize(object obj)
        {
            return JsonConvert.SerializeObject(obj, Formatting.None, new IsoDateTimeConverter() { DateTimeFormat = "yyyy-MM-dd" });
        }

        private class RuleDTO
        {
            public int BillingMethod { get; set; }
            public int? InitialAuthID { get; set; }
            public string InitialAuthCode { get; set; }
            public int? InitialMinimumMinutes { get; set; }
            public int? InitialUnitSize { get; set; }
            public int? FinalAuthID { get; set; }
            public string FinalAuthCode { get; set; }
            public int? FinalMinimumMinutes { get; set; }
            public int? FinalUnitSize { get; set; }
            public bool AllowOverlapping { get; set; }
            public bool RequiresAuthBCBA { get; set; }
            public bool RequiresPreAuthorization { get; set; }

            public static RuleDTO Create(AuthorizationMatchRule auth)
            {
                var obj = new RuleDTO()
                {
                    BillingMethod = (int)auth.BillingMethod,
                    InitialAuthID = auth.InitialAuthorizationID,
                    InitialAuthCode = auth.InitialAuthorization?.Code,
                    InitialMinimumMinutes = auth.InitialMinimumMinutes,
                    InitialUnitSize = auth.InitialUnitSize,
                    FinalAuthID = auth.FinalAuthorizationID,
                    FinalAuthCode = auth.FinalAuthorization?.Code,
                    FinalMinimumMinutes = auth.FinalMinimumMinutes,
                    FinalUnitSize = auth.FinalUnitSize,
                    AllowOverlapping = auth.AllowOverlapping,
                    RequiresAuthBCBA = auth.RequiresAuthorizedBCBA,
                    RequiresPreAuthorization = auth.RequiresPreAuthorization
                };
                return obj;
            }
        }

        private class CaseAuthDTO
        {

            public int Id { get; set; }
            public string Code { get; set; }
            public DateTime? AuthStartDate { get; set; }
            public DateTime? AuthEndDate { get; set; }

            public static CaseAuthDTO Create(Authorization caseAuth)
            {
                var obj = new CaseAuthDTO()
                {
                    Id = caseAuth.ID,
                    Code = caseAuth.AuthorizationCode?.Code,
                    AuthStartDate = caseAuth.StartDate,
                    AuthEndDate = caseAuth.EndDate
                };
                return obj;
            }
        }
    }
}
