using AABC.Data.V2;
using AABC.Domain2.PatientPortal;
using AABC.DomainServices.Utils;
using System.Collections.Generic;
using System.Linq;

namespace AABC.Web.App.Payments
{
    public class PaymentsService
    {
        private readonly CoreContext Context;

        public PaymentsService(CoreContext context)
        {
            Context = context;
        }

        public int GetPatientId(int caseID)
        {
            return Context.Cases.Single(m => m.ID == caseID).PatientID;
        }

        public IEnumerable<ListItem<int>> GetPatientLogins(int patientId)
        {
            return Context.PatientPortalLogins
                        .Where(m => m.Patients.Any(p => p.ID == patientId))
                        .OrderBy(m => m.LastName)
                        .ThenBy(m => m.FirstName)
                        .ToList()
                        .Select(m => new ListItem<int>
                        {
                            Value = m.ID,
                            Text = (m.LastName + " " + m.FirstName).Trim()
                        });
        }

        public Login GetPatientLogin(int parentLoginId)
        {
            return Context.PatientPortalLogins.SingleOrDefault(m => m.ID == parentLoginId);
        }


    }
}