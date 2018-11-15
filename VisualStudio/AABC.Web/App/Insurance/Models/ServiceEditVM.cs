using System;
using System.Collections.Generic;

namespace AABC.Web.App.Insurance.Models
{
    public class ServiceEditVM
    {

        public int? ID { get; set; }
        public int InsuranceID { get; set; }
        public int ServiceID { get; set; }
        public int ProviderTypeID { get; set; }
        public DateTime? EffectiveDate { get; set; }
        public DateTime? DefectiveDate { get; set; }

        public List<Helpers.CommonListItems.ProviderType> ProviderTypesList { get; set; }
        public List<Helpers.CommonListItems.Service> ServicesList { get; set; }

    }
}