using AABC.Domain2.Authorizations;
using AABC.Domain2.Cases;
using AABC.Domain2.Hours;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AABC.DomainServices.Hours
{
    public class AuthResolver
    {

        private Case _case;
        private Domain2.Patients.Patient _patient;
        private Domain2.Hours.Hours _hours;
        
        public AuthResolver(Domain2.Hours.Hours hours) {
            _hours = hours;
            _case = hours.Case;
            _patient = _case.Patient;
        }


        /// <summary>
        /// Update the database with the authorization breakdowns
        /// Use an existing hours context to do this
        /// </summary>
        /// <param name="context"></param>
        public void UpdateAuths(Data.V2.CoreContext context) {

            var auths = GetAuthorizationBreakdowns();
            if (_hours.AuthorizationBreakdowns != null) {
                while (_hours.AuthorizationBreakdowns.Count > 0) {
                    context.AuthorizationBreakdowns.Remove(_hours.AuthorizationBreakdowns.First());
                }
                context.SaveChanges();
            }
            _hours.AuthorizationBreakdowns = auths;
            context.SaveChanges();

        } 

        /// <summary>
        /// Generate a list of breakdowns without updating any objects or storage
        /// </summary>
        /// <returns></returns>
        public List<AuthorizationBreakdown> GetAuthorizationBreakdowns() {

            var insurance = _case.GetActiveInsuranceAtDate(_hours.Date)?.Insurance;
            //var insurance = _patient.Insurance;
            if (insurance == null) {
                return null;
            }

            var rules = insurance.AuthorizationMatchRules;
            if (rules == null || rules.Count == 0) {
                return null;
            }

            var providerType = _hours.Provider.ProviderType;
            var service = _hours.Service;

            var matchedRule = rules.Where(x => x.ServiceID == service.ID && x.ProviderTypeID == providerType.ID).FirstOrDefault();

            if (matchedRule == null) {
                return null;
            }
                        
            var breakdowns = getAuthorizationHoursBreakdowns(matchedRule);

            return breakdowns;
            
        }


        private List<AuthorizationBreakdown> getAuthorizationHoursBreakdowns(AuthorizationMatchRule matchRule) {

            var breakdowns = new List<AuthorizationBreakdown>();
            var activeCaseAuths = _case.GetActiveAuthorizations(_hours.Date);
            int totalMinutes = (int)Math.Round(_hours.TotalHours * 60);

            // make sure the basic initial match rule auth is usable
            if (!matchRule.IsInitialAuthUsableForTimeEntry) {
                return null;
            }
            
            if (totalMinutes < matchRule.InitialMinimumMinutes) {
                return null;
            }

            // make sure we actually map to one of the case's auths
            var initialAuth = activeCaseAuths.Where(x => x.AuthorizationCodeID == matchRule.InitialAuthorizationID).FirstOrDefault();

            if (initialAuth == null) {
                return null;
            }
            
            // we have a good match, create the initial breakdown
            var initialBreakdown = new AuthorizationBreakdown();

            initialBreakdown.AuthorizationID = matchRule.InitialAuthorization.ID;
            initialBreakdown.Authorization = initialAuth;
            initialBreakdown.HoursEntry = _hours;
            initialBreakdown.HoursID = _hours.ID;
            initialBreakdown.Minutes = matchRule.GetApplicableInitialMinutes(totalMinutes);

            breakdowns.Add(initialBreakdown);
            
            // make sure the final auth is usable
            if (!matchRule.IsFinalAuthUsableForTimeEntry) {
                return breakdowns;
            }
            
            var finalAuth = activeCaseAuths.Where(x => x.AuthorizationCodeID == matchRule.FinalAuthorizationID).FirstOrDefault();

            if (finalAuth == null) {
                return breakdowns;
            }

            var finalBreakdown = new AuthorizationBreakdown();
            finalBreakdown.AuthorizationID = matchRule.FinalAuthorization.ID;
            finalBreakdown.Authorization = finalAuth;
            finalBreakdown.HoursEntry = _hours;
            finalBreakdown.HoursID = _hours.ID;
            finalBreakdown.Minutes = matchRule.GetApplicableFinalMinutes(totalMinutes);

            if (finalBreakdown.Minutes > 0) {
                breakdowns.Add(finalBreakdown);
            }
            
            return breakdowns;

        }


        








    }
}
