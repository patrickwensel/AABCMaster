using System;

namespace AABC.Domain2.Referrals
{
    public class ReferralChecklistItem
    {
        public int ID { get; set; }
        public DateTime DateCreated { get; private set; }
        public string Description { get; set; }

        public ReferralChecklistItem()
        {
            DateCreated = DateTime.UtcNow;
        }
    }
}
