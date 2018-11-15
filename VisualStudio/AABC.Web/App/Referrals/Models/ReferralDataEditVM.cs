using AABC.DomainServices.Utils;
using System.Collections.Generic;

namespace AABC.Web.App.Referrals.Models
{
    public class ReferralDataEditVM
    {
        public IEnumerable<ListItem<int>> DismissalReasonTypes { get; set; }
        public IEnumerable<ListItem<int>> SourceTypes { get; set; }
        public IEnumerable<ListItem<int>> Statuses { get; set; }
        public IEnumerable<ListItem<int>> OfficeStaff { get; set; }
        public IEnumerable<ListItem<string>> Languages { get; set; }
        public IEnumerable<ListItem<string>> States { get; set; }
        public IEnumerable<ListItem<int>> InsuranceStatuses { get; set; }
        public IEnumerable<ListItem<int>> IntakeStatuses { get; set; }
        public IEnumerable<ListItem<int>> RxStatuses { get; set; }
        public IEnumerable<ListItem<int>> InsuranceCardStatuses { get; set; }
        public IEnumerable<ListItem<int>> EvaluationStatuses { get; set; }
        public IEnumerable<ListItem<int>> PolicyBookStatuses { get; set; }
        public IEnumerable<ListItem<int>> InsuranceCompanies { get; set; }
        public IEnumerable<ListItem<string>> InsuranceFundingTypes { get; set; }
        public IEnumerable<ListItem<string>> InsuranceBenefitTypes { get; set; }
        public IEnumerable<ListItem<int>> GuardianRelationships { get; set; }
    }
}