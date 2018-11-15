using AABC.Domain2.Cases;
using AABC.Domain2.Notes;
using System;
using System.Collections.Generic;

namespace AABC.Domain2.Staff
{
    public class Staff
    {
        public int ID { get; set; }
        public DateTime DateCreated { get; set; }
        //public byte[] rv { get; set; }
        public bool StaffActive { get; set; }
        public string StaffFirstName { get; set; }
        public string StaffLastName { get; set; }
        public string StaffPrimaryPhone { get; set; }
        public string StaffPrimaryEmail { get; set; }
        public DateTime? StaffHireDate { get; set; }
        public DateTime? StaffTerminatedDate { get; set; }

        public virtual ICollection<CaseNoteTask> CaseNoteTasks { get; set; }
        public virtual ICollection<ReferralNoteTask> ReferralNoteTasks { get; set; }
        public virtual ICollection<ProviderNoteTask> ProviderNoteTasks { get; set; }
        public virtual ICollection<CaseBillingCorrespondence> BillingCorrespondences { get; set; }
    }
}
