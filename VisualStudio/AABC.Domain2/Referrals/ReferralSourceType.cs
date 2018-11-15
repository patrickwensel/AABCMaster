using System;

namespace AABC.Domain2.Referrals
{
    public class ReferralSourceType
    {

        public int ID { get; set; }
        public DateTime DateCreated { get; private set; }
        public string Name { get; set; }

        public ReferralSourceType()
        {
            DateCreated = DateTime.UtcNow;
        }

    }
}
