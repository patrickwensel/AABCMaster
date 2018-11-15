using System.Collections.Generic;
using System.Linq;

namespace AABC.Web.App.Home.Models.HoursEligibility
{
    public class HoursEligiblityVM
    {

        public List<ActiveInsuranceIssue> ActiveInsuranceIssues { get; set; } = new List<ActiveInsuranceIssue>();
        public List<ActiveAuthorizationIssue> ActiveAuthorizationIssues { get; set; } = new List<ActiveAuthorizationIssue>();
        public List<ActiveRuleIssue> ActiveRuleIssues { get; set; } = new List<ActiveRuleIssue>();
        

        public static HoursEligiblityVM GetModel() {

            var context = new Data.V2.CoreContext();

            var model = new HoursEligiblityVM();

            var insuranceIssues = context.HoursEntryEligibleCasesWithoutActiveInsurance().ToList();
            var authIssues = context.HoursEntryEligibleCasesWithoutActiveAuthorizations().ToList();
            var ruleIssues = context.HoursEntryEligibleAuthorizationsWithoutMatchRules().ToList();

            model.ActiveInsuranceIssues = ActiveInsuranceIssue.Transform(insuranceIssues);
            model.ActiveAuthorizationIssues = ActiveAuthorizationIssue.Transform(authIssues);
            model.ActiveRuleIssues = ActiveRuleIssue.Transform(ruleIssues);

            return model;
        }

    }


    public class ActiveInsuranceIssue
    {
        public int CaseID { get; set; }
        public string PatientName { get; set; }

        public static List<ActiveInsuranceIssue> Transform(List<Domain2.Cases.Case> cases) {

            var items = new List<ActiveInsuranceIssue>();

            foreach (var c in cases) {
                items.Add(new ActiveInsuranceIssue()
                {
                    CaseID = c.ID,
                    PatientName = c.Patient.CommonName
                });
            }

            return items;
        }
    }

    public class ActiveAuthorizationIssue
    {
        public int CaseID { get; set; }
        public string PatientName { get; set; }

        public static List<ActiveAuthorizationIssue> Transform(List<Domain2.Cases.Case> cases) {
            var items = new List<ActiveAuthorizationIssue>();
            foreach (var c in cases) {
                items.Add(new ActiveAuthorizationIssue()
                {
                    CaseID = c.ID,
                    PatientName = c.Patient.CommonName
                });
            }
            return items;
        }
    }

    public class ActiveRuleIssue
    {
        public int AuthorizationID { get; set; }
        public int CaseID { get; set; }
        public string PatientName { get; set; }
        public string AuthorizationCode { get; set; }

        public static List<ActiveRuleIssue> Transform(List<Domain2.Authorizations.Authorization> auths) {
            var items = new List<ActiveRuleIssue>();
            foreach (var a in auths) {
                items.Add(new ActiveRuleIssue()
                {
                    AuthorizationCode = a.AuthorizationCode.Code,
                    CaseID = a.CaseID,
                    PatientName = a.Case.Patient.CommonName,
                    AuthorizationID = a.ID
                });
            }
            return items;
        }
    }

}