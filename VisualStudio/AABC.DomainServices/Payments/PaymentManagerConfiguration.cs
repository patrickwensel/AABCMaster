using System.Configuration;

namespace AABC.DomainServices.Payments
{
    public class PaymentManagerConfiguration
    {
        //days
        public int OneTimeTransactionTimeWindowWarning { get; set; }
        //days
        public int OneTimeTransactionTimeWindow { get; set; }
        //months
        public int RecurringTransactionTimeWindow { get; set; }

        public static PaymentManagerConfiguration Create()
        {
            return new PaymentManagerConfiguration
            {
                OneTimeTransactionTimeWindowWarning = int.Parse(ConfigurationManager.AppSettings["PaymentManager.OneTimeTransactionTimeWindowWarning"]),
                OneTimeTransactionTimeWindow = int.Parse(ConfigurationManager.AppSettings["PaymentManager.OneTimeTransactionTimeWindow"]),
                RecurringTransactionTimeWindow = int.Parse(ConfigurationManager.AppSettings["PaymentManager.RecurringTransactionTimeWindow"])
            };
        }

        public static PaymentManagerConfiguration CreateDefault()
        {
            return new PaymentManagerConfiguration
            {
                OneTimeTransactionTimeWindowWarning = 30,
                OneTimeTransactionTimeWindow = 90,
                RecurringTransactionTimeWindow = 12
            };
        }
    }
}
