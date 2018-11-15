using System;
namespace AABC.PatientPortal.App.Payments.Models
{
    public class CasePaymentPlanVM
    {

        public int Id { get; set; }

        public int CaseId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal Amount { get; set; }
        public string Frequency { get; set; }
        public bool Active { get; set; }


    }
}