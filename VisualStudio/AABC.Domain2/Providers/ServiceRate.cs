using System;

namespace AABC.Domain2.Providers
{
    public class ServiceRate 
    {


        public int ID { get; set; }
        public int ProviderID { get; set; }
        public int ServiceID { get; set; }
        public RateType Type { get { return RateType.Hourly; } }
        public decimal Rate { get; set; }
        public DateTime EffectiveDate { get; set; }

        public virtual Provider Provider { get; set; }
        public virtual Services.Service Service { get; set; }
    }
}
