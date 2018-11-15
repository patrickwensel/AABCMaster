using System.Collections.Generic;
using System.Linq;

namespace AABC.Web.App.Reporting.Models
{
    public class AuthsUnmatchedDetailVM
    {

        

        public int AuthCodeID { get; set; }
        public string AuthCode { get; set; }

        public int PatientID { get; set; }
        public int CaseID { get; set; }
        public string PatientName { get; set; }

        public int? InsuranceID { get; set; }
        public string InsuranceName { get; set; }
        
        public List<Domain2.Authorizations.AuthorizationMatchRule> MatchRules { get; set; }        
        public List<Domain2.Authorizations.Authorization> Authorizations { get; set; }

        public List<string> MatchRuleDisplayList {
            get {
                if (MatchRules == null || MatchRules.Count == 0) {
                    return null;
                }
                var list = new List<string>();

                foreach (var rule in MatchRules.OrderBy(x => x.ProviderType.Code).ThenBy(x => x.Service.Code)) {

                    string s = rule.ProviderType.Code + ", " + rule.Service.Code + " - [Initial: ";

                    if (rule.InitialAuthorization == null) {
                        s += "none] ";
                    } else {
                        s += rule.InitialAuthorization.Code + " - " + rule.InitialMinimumMinutes + "/" + rule.InitialUnitSize + "] ";
                    }

                    s += "[Final: ";

                    if (rule.FinalAuthorization == null) {
                        s += "none]";
                    } else {
                        s += rule.FinalAuthorization.Code + " - " + rule.FinalMinimumMinutes + "/" + rule.FinalUnitSize + "]";
                    }

                    list.Add(s);

                }

                return list;
            }
        }

    }
}