using System;

namespace AABC.Domain2.Insurances
{
    public class InsuranceService
    {

        public int ID { get; set; }
        public int InsuranceID { get; set; }
        public int ServiceID { get; set; }
        public int ProviderTypeID { get; set; }

        public DateTime? EffectiveDate { get; set; }
        public DateTime? DefectiveDate { get; set; }

        public Insurance Insurance { get; set; }
        public Services.Service Service { get; set; }
        public Providers.ProviderType ProviderType { get; set; }


    }
}
