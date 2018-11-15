using AABC.Data.V2;
using AABC.Domain2.Cases;
using AABC.DomainServices.Utils;
using AABC.PatientPortal.App.Payments.Models;
using System.Collections.Generic;
using System.Linq;

namespace AABC.PatientPortal.App.Payments
{
    public class PaymentsService
    {
        private readonly CoreContext _context;

        public PaymentsService()
        {
            _context = AppService.Current.Data.Context;
        }


        public CasePaymentPlanVM GetActivePlanByUser(int UserId)
        {
            var plan = _context.CasePaymentPlans.Where(cpp => cpp.Case.Patient.PatientPortalLogins.Where(l => l.ID == UserId).Any() && cpp.Active == true).OrderByDescending(cpp => cpp.EndDate).FirstOrDefault();
            if (plan != null)
            {
                return Transform(plan);
            }
            else
            {
                return null;
            }
        }

        public IEnumerable<ListItem<int>> GetPatients(int LoginId)
        {
            var patients = _context.PatientPortalLogins
                                    .Where(l => l.WebMembershipDetail.ID == LoginId)
                                    .SelectMany(l => l.Patients)
                                    .OrderBy(m => m.LastName)
                                    .ToList();
            return patients.Select(m => new ListItem<int>
            {
                Value = m.ID,
                Text = m.LastName + " " + m.FirstName
            });
        }


        private CasePaymentPlanVM Transform(CasePaymentPlan n)
        {
            var r = new CasePaymentPlanVM();
            r.Active = n.Active;
            r.Amount = n.Amount;
            r.CaseId = n.CaseId;
            r.EndDate = n.EndDate;
            r.Frequency = n.Frequency;
            r.Id = n.Id;
            r.StartDate = n.StartDate;
            return r;
        }

    }
}
