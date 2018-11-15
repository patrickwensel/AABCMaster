using AABC.Domain2.Payments;
using System;
using System.Collections.Generic;

namespace AABC.DomainServices.Payments.Repositories
{
    public interface IPaymentRepository : IRepository<Payment>
    {
        IEnumerable<Payment> GetScheduledPaymentsByLoginId(int loginId, DateTime date);
        IEnumerable<Payment> GetScheduledPaymentsByPatientId(int patientId, DateTime date);
    }
}