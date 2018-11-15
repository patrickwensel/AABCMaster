using System;

namespace AABC.Domain2.Referrals
{
    public class ReferralChecklist
    {
        public int ID { get; set; }
        public DateTime DateCreated { get; private set; }
        public int ReferralID { get; set; }
        public int ChecklistItemID { get; set; }
        public bool IsComplete { get; set; }
        public int? CompletedByStaffID { get; set; }

        public virtual Referral Referral { get; set; }
        public virtual ReferralChecklistItem ChecklistItem { get; set; }
        public virtual Staff.Staff CompletedByStaff { get; set; }


        public ReferralChecklist()
        {
            DateCreated = DateTime.UtcNow;
        }
    }
}
