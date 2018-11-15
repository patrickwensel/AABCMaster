using AABC.Domain2.Payments;
using System;
using System.Collections.Generic;

namespace AABC.DomainServices.Payments
{
    public class DatesGenerator
    {

        public IEnumerable<DateTime> GetDates(DateTime start, DateTime end, RecurringFrequency frequency)
        {
            var dates = new List<DateTime>();
            while (start <= end)
            {
                dates.Add(start);
                switch (frequency)
                {
                    case RecurringFrequency.weekly:
                        start = start.AddDays(7);
                        break;
                    case RecurringFrequency.monthly:
                        start = start.AddMonths(1);
                        break;
                    default:
                        throw new InvalidOperationException();
                }
            }
            return dates;
        }
    }
}
