using System;

namespace AABC.Domain2.Infrastructure
{
    public class DefaultDateTimeProvider : IDateTimeProvider
    {
        public DateTime Now {
            get
            {
                return DateTime.Now;
            }
        }

        public DateTime UtcNow {
            get
            {
                return DateTime.UtcNow;
            }
        }
    }
}
