using System;

namespace AABC.DomainServices.Utils
{
    public static class AgeCalculator
    {
        public static int Age(DateTime birthday)
        {
            DateTime now = DateTime.Today;
            int age = now.Year - birthday.Year;
            if (now < birthday.AddYears(age)) age--;
            return age;
        }
    }
}
