using System;

namespace AABC.Domain2.Providers
{
    public class ProviderInsuranceCredential
    {
        public int ID { get; set; }
        public int ProviderID { get; set; }
        public int InsuranceID { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public virtual Insurances.Insurance Insurance { get; set; }
        public virtual Provider Provider { get; set; }
    }
}
