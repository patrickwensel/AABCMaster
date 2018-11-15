using System;

namespace AABC.Web.Models.Providers
{
    public class CaseHoursListItemVM
    {

        public int HoursID { get; set; }
        public int CaseID { get; set; }
        public int? CaseAuthID { get; set; }
        public Domain.Cases.AuthorizationHoursStatus StatusID { get; set; }

        public string DoneLink { get { return "dummayValue"; } }
        
        public string StatusCode { get; set; }
        public bool Billed { get; set; }
        public bool Paid { get; set; }
        public string PatientName { get; set; }
        public string AuthCode { get; set; }
        public Models.Cases.TimeBillGridAuthItemVM CaseAuth { get; set; }
        public DateTime Date { get; set; }
        public DateTime TimeIn { get; set; }
        public DateTime TimeOut { get; set; }
        public Domain.Cases.Service Service { get; set; }
        public double Hours { get; set; }
        public double? Payable { get; set; }
        public double? Billable { get; set; }
        public string Notes { get; set; }
        public bool HasCatalystData { get; set; }

    }
}