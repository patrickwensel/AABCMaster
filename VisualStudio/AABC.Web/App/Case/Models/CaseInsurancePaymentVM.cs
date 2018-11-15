using System.Collections.Generic;
namespace AABC.Web.Models.Cases
{
    public class CaseInsurancePaymentVM
    {

        public Dymeng.Framework.Web.Mvc.Views.IViewModelBase Base;

        public int Id { get; set; }
        public int PaymentCaseInsuranceId { get; set; }
        public string PaymentCaseInsuranceName { get; set; }
        public decimal InsurancePaymentAmount { get; set; }
        public System.DateTime PaymentDate { get; set; }
        public string Description { get; set; }

        public List<CaseInsuranceVM> CaseInsuranceList { get; set; }

    }
}