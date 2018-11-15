using AABC.DomainServices.Payments;

namespace AABC.Web.App.Payments.Models
{
    public class ExtendedPaymentCreationParameters : PaymentCreationParameters
    {
        public int PatientLoginId { get; set; }
    }
}