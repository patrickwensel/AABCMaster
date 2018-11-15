using System;
using System.Collections.Generic;

namespace AABC.Scheduling.Contracts
{
    public interface IOccurrencesGenerator
    {
        IEnumerable<DateTime> CreateWeeklyOccurrencies(DateTime startDate, DateTime endDate, DayOfWeek dayOfWeek);
    }

}
