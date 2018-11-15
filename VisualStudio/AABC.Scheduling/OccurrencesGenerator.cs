using AABC.Scheduling.Contracts;
using System;
using System.Collections.Generic;

namespace AABC.Scheduling
{
    public class OccurrencesGenerator : IOccurrencesGenerator
    {
        public IEnumerable<DateTime> CreateWeeklyOccurrencies(DateTime startDate, DateTime endDate, DayOfWeek dayOfWeek)
        {
            if (startDate.Date > endDate.Date)
            {
                throw new ArgumentOutOfRangeException($"StartDate '{startDate}' cannot be greater than EndDate '{endDate}'.");
            }
            var ocurrences = new List<DateTime>();
            var nextOcurrence = DateUtils.GetNextDayOfWeek(startDate, dayOfWeek);
            while (nextOcurrence <= endDate.Date)
            {
                ocurrences.Add(nextOcurrence);
                nextOcurrence = nextOcurrence.AddDays(7);
            }
            return ocurrences;
        }


    }
}
