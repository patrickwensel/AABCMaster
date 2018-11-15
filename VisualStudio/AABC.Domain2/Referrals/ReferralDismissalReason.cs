using System;

namespace AABC.Domain2.Referrals
{
    public class ReferralDismissalReason
    {
        public int ID { get; set; }
        public DateTime DateCreated { get; private set; }
        public string Code { get; set; }
        public string Name { get; set; }

        public ReferralDismissalReason()
        {
            DateCreated = DateTime.UtcNow;
        }
    }
}
