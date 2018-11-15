using System;

namespace AABC.Scheduling
{
    public static class DateUtils
    {

        public static DateTime GetNextDayOfWeek(DateTime start, DayOfWeek dayOfWeek)
        {
            int daysUntilDayOfWeek = ((int)dayOfWeek - (int)start.DayOfWeek + 7) % 7;
            DateTime nextDayOfWeek = start.AddDays(daysUntilDayOfWeek);
            return nextDayOfWeek;
        }
    }
}
