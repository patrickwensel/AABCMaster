using System;
using System.Collections.Generic;

namespace AABC.Domain.Cases
{

    public class MonthlyCasePeriod
    {
        public int? ID { get; set; }
        public DateTime FirstDayOfMonth { get; set; }

        List<ProviderHoursMonthFinalization> ProviderHoursMonthFinalizations { get; set; }
        
    }
}
