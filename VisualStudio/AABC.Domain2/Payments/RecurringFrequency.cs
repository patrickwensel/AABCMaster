using System.ComponentModel;

namespace AABC.Domain2.Payments
{
    public enum RecurringFrequency
    {
        [Description("Weekly")]
        weekly,
        [Description("Monthly")]
        monthly
    }
}
