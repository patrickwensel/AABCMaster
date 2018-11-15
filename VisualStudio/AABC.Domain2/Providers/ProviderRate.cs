using System;

namespace AABC.Domain2.Providers
{



    public class ProviderRate
    {
        public int ID { get; set; }
        public int ProviderID { get; set; }
        public RateType Type { get; set; }
        public decimal Rate { get; set; }
        public DateTime EffectiveDate { get; set; } = new DateTime(1900, 1, 1);

        public virtual Provider Provider { get; set; }

    }
}
