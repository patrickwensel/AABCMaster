using AABC.Domain2.Payments;
using System.Collections.Generic;

namespace AABC.DomainServices.Payments.Repositories
{
    public interface IPaymentChargeRepository : IRepository<PaymentCharge>
    {
        IEnumerable<PaymentCharge> GetChargesByPatientId(int patientId);
        IEnumerable<PaymentCharge> GetChargesByLoginId(int loginId);
    }
}