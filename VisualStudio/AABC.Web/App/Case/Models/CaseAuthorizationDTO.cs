using System;

namespace AABC.Web.App.Case.Models
{
    public class CaseAuthorizationDTO
    {
        public int ID { get; set; }
        public string Status { get; set; }
        public string InsuranceName { get; set; }
        public string Type { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal TotalHoursApproved { get; set; }
    }
}