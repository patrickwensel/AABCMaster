using AABC.Domain2.Payments;
using AABC.DomainServices.Payments.Models;
using AABC.DomainServices.Utils;
using System.Linq;

namespace AABC.DomainServices.Payments
{
    public static class Mapper
    {
        public static Payment ToPayment(PaymentCreationParameters source, Payment destination)
        {
            destination.PaymentType = source.PaymentType;
            destination.PatientId = source.PatientId;
            destination.Amount = source.Amount;
            if (source.PaymentType == PaymentType.onetime)
            {
                destination.OneTimePaymentDate = source.OneTimePaymentDate;
            }
            else if (source.PaymentType == PaymentType.recurring)
            {
                destination.RecurringFrequency = source.RecurringFrequency;
                destination.RecurringDateEnd = source.RecurringDateEnd;
                destination.RecurringDateStart = source.RecurringDateStart;
            }
            return destination;
        }

        public static CreditCard ToCreditCard(PaymentCreationParameters source, CreditCard destination)
        {
            destination.Cardholder = source.CardHolder;
            destination.CardNumber = source.CardNumber.Substring(source.CardNumber.Length - 4);
            destination.ExpiryMonth = source.CardExpiryMonth;
            destination.ExpiryYear = source.CardExpiryYear;
            return destination;
        }


        public static PaymentChargeDTO ToPaymentChargeDTO(PaymentCharge n)
        {
            var r = new PaymentChargeDTO();
            r.Id = n.Id;
            r.Description = n.Description;
            r.PatientName = n.Payment.Patient.CommonName;
            r.Amount = n.Amount;
            r.ChargeDate = n.ChargeDate;
            r.IsPatientGenerated = !n.Payment.ManagementUserId.HasValue;
            return r;
        }

        public static PaymentWithScheduledDTO ToPaymentWithScheduledDTO(Payment payment)
        {
            var paymentVM = new PaymentWithScheduledDTO();
            paymentVM.Id = payment.Id;
            paymentVM.PatientId = payment.PatientId;
            paymentVM.PatientName = payment.Patient.CommonName;
            paymentVM.PaymentType = EnumHelper.GetDescription(payment.PaymentType);
            paymentVM.RecurringFrequency = payment.RecurringFrequency.HasValue ? EnumHelper.GetDescription(payment.RecurringFrequency.Value) : string.Empty;
            paymentVM.Active = payment.Active;
            paymentVM.Schedules = payment.PaymentSchedules
                    .OrderBy(m => m.ScheduledDate)
                    .Select(n =>
                    {
                        var paymentScheduleVM = new PaymentScheduleDTO();
                        paymentScheduleVM.Id = n.Id;
                        paymentScheduleVM.ScheduledDate = n.ScheduledDate;
                        paymentScheduleVM.Amount = n.Payment.Amount;
                        paymentScheduleVM.PaymentChargeId = n.PaymentChargeId;
                        return paymentScheduleVM;
                    });
            return paymentVM;
        }
    }
}
