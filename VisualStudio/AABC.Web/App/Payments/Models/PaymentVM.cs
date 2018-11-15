using AABC.Domain2.Payments;
using AABC.DomainServices.Payments.Models;
using AABC.DomainServices.Utils;
using System;
using System.Collections.Generic;

namespace AABC.Web.App.Payments.Models
{
    public class PaymentVM : BasePaymentVM<ExtendedPaymentCreationParameters>
    {
        public IEnumerable<ListItem<int>> PatientLogins { get; set; }

        public PaymentVM()
        {
            Data = new ExtendedPaymentCreationParameters();
            Data.PaymentType = PaymentType.recurring;
            Data.RecurringFrequency = RecurringFrequency.monthly;
            Data.RecurringDateStart = DateTime.Now.Date;
            Data.RecurringDateEnd = DateTime.Now.Date.AddMonths(1);
            Data.OneTimePaymentDate = DateTime.Now.Date;
        }
    }
}