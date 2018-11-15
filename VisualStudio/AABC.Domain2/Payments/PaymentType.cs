using System.ComponentModel;

namespace AABC.Domain2.Payments
{
    public enum PaymentType
    {
        [Description("One Time")]
        onetime,
        [Description("Recurring")]
        recurring
    }
}
