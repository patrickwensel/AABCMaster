using System;

namespace AABC.Domain.Cases
{
    public class CaseAuthorizationGeneralHours
    {
        public int? ID { get; set; }
        public DateTime? DateCreated { get; set; }
        public int? CaseAuthID { get; set; } // ID of the parent auth, filled if required

        public int Year { get; set; }
        public int Month { get; set; }
        public double Hours { get; set; }

        public DateTime FirstDayOfMonth {
            get
            {
                return new DateTime(Year, Month, 1);

            }
        }

    }
}
