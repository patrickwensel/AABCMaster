using System;

namespace AABC.Web.App.Case.Models
{
    public class CaseInsuranceHistoryDTO
    {
        public int Id { get; set; }
        public string InsuranceName { get; set; }
        public string MemberId { get; set; }
        public DateTime? DatePlanEffective { get; set; }
        public DateTime? DatePlanTerminated { get; set; }
        public string OtherNotes { get; set; }
    }
}