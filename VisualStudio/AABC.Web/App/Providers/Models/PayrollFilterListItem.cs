using System.Collections.Generic;

namespace AABC.Web.App.Providers.Models
{
    public enum PayrollFilter
    {
        None = 0,
        NYOnly = 1,
        NonNY = 2
    }
    

    public class PayrollFilterListItem
    {
        public string Display { get; set; }
        public string Value { get; set; }
        public PayrollFilter EnumValue { get; set; }

        public static PayrollFilter GetEnumValue(string filter) {
            switch (filter) {
                case "ny-only": return PayrollFilter.NYOnly;
                case "non-ny": return PayrollFilter.NonNY;
            }
            return PayrollFilter.None;
        }

        public static List<PayrollFilterListItem> GetList() {

            return new List<PayrollFilterListItem>()
            {
                new PayrollFilterListItem() { Display = "No Filter", Value = "none", EnumValue = PayrollFilter.None},
                new PayrollFilterListItem() { Display = "NY Only", Value = "ny-only", EnumValue = PayrollFilter.NYOnly},
                new PayrollFilterListItem() { Display = "Non-NY", Value = "non-ny", EnumValue = PayrollFilter.NonNY}
            };
        }
    }
}