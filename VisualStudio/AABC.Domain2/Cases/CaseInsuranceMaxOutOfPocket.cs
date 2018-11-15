using System;

namespace AABC.Domain2.Cases
{
    public class CaseInsuranceMaxOutOfPocket
    {
        public int Id { get; set; }
        public int CaseInsuranceId { get; set; }
        public Nullable<decimal> MaxOutOfPocket { get; set; }
        public Nullable<System.DateTime> EffectiveDate { get; set; }

        public virtual CaseInsurance CaseInsurance { get; set; }
    }
}
