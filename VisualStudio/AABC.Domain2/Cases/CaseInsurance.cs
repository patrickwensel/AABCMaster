using AABC.Domain2.Insurances;
using System;
using System.Collections.Generic;

namespace AABC.Domain2.Cases
{
    public class CaseInsurance
    {
        public int ID { get; set; }

        public int CaseID { get; set; }
        public int? InsuranceID { get; set; }
        public int? CarrierID { get; set; }
        public string MemberId { get; set; }
        public string MemberName { get; set; }
        public string PrimaryCardholderName { get; set; }
        public DateTime? PrimaryCardholderDOB { get; set; }
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
        public bool PaymentPlanLikeToDiscuss { get; set; }
        public DateTime? PaymentPlanStartDate { get; set; }
        public decimal? PaymentPlanMonthlyAmount { get; set; }
        public string PaymentPlanMethodOfPayment { get; set; }
        public virtual Insurance Insurance { get; set; }
        public virtual Case Case { get; set; }
        public virtual ICollection<CaseInsuranceMaxOutOfPocket> CaseInsurancesMaxOutOfPocket { get; set; } = new List<CaseInsuranceMaxOutOfPocket>();
        public virtual ICollection<CaseInsurancePayment> CaseInsurancePayments { get; set; } = new List<CaseInsurancePayment>();
        public virtual LocalCarrier Carrier { get; set; }
    }
}
