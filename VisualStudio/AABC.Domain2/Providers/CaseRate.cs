using System;

namespace AABC.Domain2.Providers
{
    public class CaseRate
    {
        
        public int ID { get; set; }
        public int ProviderID { get; set; }
        public int CaseID { get; set; }
        public RateType Type { get; set; }
        public decimal Rate { get; set; }
        public DateTime EffectiveDate { get; set; }

        public virtual Provider Provider { get; set; }        
        public virtual Cases.Case Case { get; set; }

    }
}
