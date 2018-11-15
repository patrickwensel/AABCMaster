using System;

namespace AABC.Domain.Cases
{
    public class ProviderHoursMonthFinalization
    {

        public int? ID { get; set; }

        public int? CaseID { get; set; }
        public int? ProviderID { get; set; }

        public Case Case { get; set; }
        public Providers.Provider Provider { get; set; }

        public DateTime DateFinalized { get; set; }

    }
}
