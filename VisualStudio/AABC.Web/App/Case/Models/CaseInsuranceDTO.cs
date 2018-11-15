using System;
using System.Collections.Generic;

namespace AABC.Web.App.Case.Models
{
    public class CaseInsuranceDTO
    {
        public int Id { get; set; }
        public int InsuranceCaseId { get; set; }
        public int? InsuranceId { get; set; }
        public int? InsuranceCarrierId { get; set; }
        public string MemberName { get; set; }
        public string MemberId { get; set; }
        public string PrimaryCardholderName { get; set; }
        public DateTime? DatePlanEffective { get; set; }
        public DateTime? DatePlanTerminated { get; set; }
        public string FundingType { get; set; }
        public string BenefitType { get; set; }
        public decimal? CoPayAmount { get; set; }
        public decimal? CoInsuranceAmount { get; set; }
        public decimal? DeductibleTotal { get; set; }
        public string OtherNotes { get; set; }
        public bool HardshipWaiverLike { get; set; }
        public bool HardshipWaiverApplied { get; set; }
        public bool HardshipWaiverApproved { get; set; }

        public IEnumerable<CaseInsuranceMaxOutOfPocketDTO> MaxOutOfPocket { get; set; }

    }
}