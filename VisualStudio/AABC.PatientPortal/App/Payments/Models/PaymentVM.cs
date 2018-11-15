using AABC.Domain2.Payments;
using AABC.DomainServices.Payments;
using AABC.DomainServices.Payments.Models;
using AABC.DomainServices.Utils;
using System;
using System.Collections.Generic;

namespace AABC.PatientPortal.App.Payments.Models
{
    public class PaymentVM : BasePaymentVM<PaymentCreationParameters>
    {
        public IEnumerable<ListItem<int>> Patients { get; set; }
        public CasePaymentPlanVM ActivePlan { get; set; }

        public PaymentVM()
        {
            Data = new PaymentCreationParameters();
            Data.PaymentType = PaymentType.recurring;
            Data.RecurringFrequency = RecurringFrequency.monthly;
            Data.RecurringDateStart = DateTime.Now.Date;
            Data.RecurringDateEnd = DateTime.Now.Date.AddMonths(1);
            Data.OneTimePaymentDate = DateTime.Now.Date;
        }
    }
}