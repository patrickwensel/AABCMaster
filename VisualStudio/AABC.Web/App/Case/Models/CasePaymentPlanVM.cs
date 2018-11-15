using System;
namespace AABC.Web.Models.Cases
{
    public class CasePaymentPlanVM
    {
        public Dymeng.Framework.Web.Mvc.Views.IViewModelBase Base;
        public int Id { get; set; }
        public int PaymentPlanCaseId { get; set; }
        public DateTime PaymentPlanStartDate { get; set; }
        public DateTime PaymentPlanEndDate { get; set; }
        public decimal Amount { get; set; }
        public string Frequency { get; set; }
        public bool PaymentPlanActive { get; set; }
        public int PatientID { get; set; }
        public string PatientName { get; set; }
        public string PatientGender { get; set; }
    }
}