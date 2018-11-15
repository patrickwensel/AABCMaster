using System;
using System.Collections.Generic;

namespace AABC.Domain2.Cases
{
    public class ParentApproval
    {

        public int ID { get; set; }

        public int PeriodID { get; set; }
        public int ParentLoginID { get; set; }
        public DateTime DateApproved { get; set; }

        public virtual MonthlyPeriod Period { get; set; }
        public virtual PatientPortal.Login ParentLogin { get; set; }
        public virtual ICollection<Hours.Hours> Hours { get; set; }


    }
}
