using AABC.Domain2.Payments;
using System;
using System.Collections.Generic;

namespace AABC.DomainServices.Payments.Repositories
{
    public interface IPaymentScheduleRepository : IRepository<PaymentSchedule>
    {
        IEnumerable<PaymentSchedule> GetUnpaidSchedules(DateTime date);
    }
}
