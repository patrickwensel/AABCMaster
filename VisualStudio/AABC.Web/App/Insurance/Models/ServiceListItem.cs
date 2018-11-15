using System;

namespace AABC.Web.App.Insurance.Models
{
    public class ServiceListItem
    {

        public int ID { get; set; }
        public int ServiceID { get; set; }
        public string ServiceCode { get; set; }
        public string ServiceName { get; set; }
        public int ProviderTypeID { get; set; }
        public string ProviderTypeCode { get; set; }
        public DateTime? EffectiveDate { get; set; }
        public DateTime? DefectiveDate { get; set; }

    }
}