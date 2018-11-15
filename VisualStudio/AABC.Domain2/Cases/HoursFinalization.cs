using System;

namespace AABC.Domain2.Cases
{
    public class HoursFinalization
    {

        public int ID { get; set; }
        public DateTime DateCreated { get; set; } = DateTime.Now;
        public int PeriodID { get; set; }
        public int ProviderID { get; set; }
        public DateTime DateFinalized { get; set; }
        public int IsComplete { get; set; }
        public string EnvelopeID { get; set; }

        public virtual Providers.Provider Provider { get; set; }
        public virtual MonthlyPeriod Period { get; set; }

    }
}
