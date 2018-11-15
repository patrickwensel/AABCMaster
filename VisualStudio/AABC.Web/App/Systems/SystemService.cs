using AABC.Domain.Email;
using AABC.DomainServices.Payments.Gateways;
using System;
using System.ComponentModel.DataAnnotations;

namespace AABC.Web.App.Systems
{
    public class SystemService
    {
        private readonly StripeForRBT StripeService;
        private readonly Mailer Mailer;
        private readonly ITransactionLogger TransactionLogger;

        public SystemService(
            StripeForRBT stripeService,
            Mailer mailer,
            ITransactionLogger transactionLogger
        )
        {
            if (stripeService == null) throw new ArgumentNullException(nameof(stripeService));
            if (mailer == null) throw new ArgumentNullException(nameof(mailer));
            StripeService = stripeService;
            Mailer = mailer;
            TransactionLogger = transactionLogger;
        }

        public object ProcessPayment(PaymentParameters parameters, string description, int amount)
        {
            var month = parameters.ExpiryDate.Substring(0, 2);
            var year = parameters.ExpiryDate.Substring(2, 2);
            var result = StripeService.Charge(description, amount, parameters.Name, parameters.CreditCard, month, year, parameters.CVC, parameters.Email);
            if (result.Success)
            {
                Mailer.SendNotification(parameters.Name, parameters.Phone, parameters.Email);
            }
            Log(parameters, result);
            return result;
        }

        private void Log(PaymentParameters parameters, PaymentTransactionResult result)
        {
            if (TransactionLogger != null)
            {
                TransactionLogger.Log(new TransactionLogEntry
                {
                    Name = parameters.Name,
                    Phone = parameters.Phone,
                    Email = parameters.Email,
                    TransactionId = result.Success ? result.TransactionId : string.Empty
                });
            }
        }

        public static SystemService Create(string apiKey, string apiEndPoint, SMTPAccount smtpAccount, string recipient, string connectionString)
        {
            return new SystemService(new StripeForRBT(apiKey, apiEndPoint), new Mailer(smtpAccount, recipient), new TransactionLogger(connectionString));
        }
    }

    public class PaymentParameters
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string Phone { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Credit Card must be numeric")]
        public string CreditCard { get; set; }

        [Required]
        [StringLength(4, MinimumLength = 4, ErrorMessage = "Expiry Date must be formatted MMYY")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Expiry Date must be numeric")]
        public string ExpiryDate { get; set; }

        [Required]
        public string CVC { get; set; }
    }
}