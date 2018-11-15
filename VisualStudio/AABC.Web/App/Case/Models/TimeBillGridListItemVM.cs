using System;

namespace AABC.Web.Models.Cases
{
    public class TimeBillGridListItemVM
    {

        public int HoursID { get; set; }
        public int ProviderID { get; set; }
        public int? CaseAuthID { get; set; }
        public Domain.Cases.AuthorizationHoursStatus StatusID { get; set; }

        public string DoneLink { get { return "dummyValue"; } }

        public string StatusCode { get; set; }
        public bool Billed { get; set; }
        public bool Paid { get; set; }
        public string ProviderName { get; set; }
        public string ProviderTypeName { get; set; }
        public int ProviderTypeID { get; set; }
        public string AuthCode { get; set; }
        //public Domain.Cases.CaseAuthorization CaseAuthorization { get; set; }
        public TimeBillGridAuthItemVM CaseAuth { get; set; }
        public DateTime Date { get; set; }
        public DateTime TimeIn { get; set; }
        public DateTime TimeOut { get; set; }
        public Domain2.Services.Service Service { get; set; }
        public double Hours { get; set; }
        public double? Payable { get; set; }
        public double? Billable { get; set; }
        public string Notes { get; set; }
        public bool HasCatalystData { get; set; }
        public bool Reported { get; set; }
        public bool PreCheckedNoSession { get; set; }
        public Domain.Services.ServiceLocation ServiceLocation { get; set; }
        public int? ServiceLocationID { get; set; }

    }
}