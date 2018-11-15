using AABC.Domain2.Payments;
using AABC.DomainServices.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace AABC.DomainServices.Payments.Models
{
    public abstract class BasePaymentVM<TData>
        where TData : PaymentCreationParameters
    {
        public IEnumerable<ListItem<int>> PaymentTypes { get; private set; }
        public IEnumerable<ListItem<int>> Frequencies { get; private set; }
        public IEnumerable<ListItem<string>> Months { get; private set; }
        public IEnumerable<ListItem<string>> Years { get; private set; }
        public PaymentManagerConfiguration Configuration { get; set; }
        public TData Data { get; set; }

        public BasePaymentVM()
        {
            PaymentTypes = Enum.GetValues(typeof(PaymentType))
                                .Cast<PaymentType>()
                                .Select(m => new ListItem<int>
                                {
                                    Value = (int)m,
                                    Text = EnumHelper.GetDescription(m)
                                });
            Frequencies = Enum.GetValues(typeof(RecurringFrequency))
                                .Cast<RecurringFrequency>()
                                .Select(m => new ListItem<int>
                                {
                                    Value = (int)m,
                                    Text = EnumHelper.GetDescription(m)
                                });
            Months = DateTimeFormatInfo.CurrentInfo.MonthNames
                            .Where(m => !string.IsNullOrEmpty(m))
                            .Select((m, i) => new ListItem<string>
                            {
                                Value = (i + 1).ToString("00"),
                                Text = (i + 1).ToString("00") + " - " + m
                            });
            Years = Enumerable.Range(DateTime.Now.Year, 10).Select(m => new ListItem<string>
            {
                Value = m.ToString(),
                Text = m.ToString()
            });
        }
    }
}
