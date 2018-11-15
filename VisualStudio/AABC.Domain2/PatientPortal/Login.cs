using System;
using System.Collections.Generic;
using System.Linq;

namespace AABC.Domain2.PatientPortal
{
    public class Login
    {


        public int ID { get; set; }
        public DateTime DateCreated { get; set; }

        public bool Active { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Password { get; set; }
        
        public virtual WebMembershipDetail WebMembershipDetail { get; set; }

        public virtual ICollection<Patients.Patient> Patients { get; set; }
        public virtual ICollection<Cases.ParentApproval> CasePeriodApprovals { get; set; }
        public virtual ICollection<ParentSignature> Signatures { get; set; }
        public virtual ICollection<AcceptedTerms> AcceptedTerms { get; set; }
        public virtual ICollection<Hours.ReportLogItem> ReportLogs { get; set; }
        public virtual ICollection<Payments.Payment> Payments { get; set; }
        public virtual ICollection<Payments.CreditCard> CreditCards { get; set; }


        public ParentSignature GetCurrentSignature() {
            if (Signatures == null || Signatures.Count == 0) {
                return null;
            }

            var sig = Signatures.OrderByDescending(x => x.Date).FirstOrDefault();

            return sig;
        }

    }
}
