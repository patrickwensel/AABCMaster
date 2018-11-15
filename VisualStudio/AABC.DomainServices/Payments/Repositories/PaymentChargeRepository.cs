using AABC.Data.V2;
using AABC.Domain2.Payments;
using System.Collections.Generic;
using System.Linq;

namespace AABC.DomainServices.Payments.Repositories
{
    public class PaymentChargeRepository : BaseRepository<PaymentCharge>, IPaymentChargeRepository
    {
        public PaymentChargeRepository(CoreContext context) : base(context)
        {
        }

        public override PaymentCharge Create()
        {
            return _context.PaymentCharges.Create();
        }

        public override PaymentCharge GetById(int id)
        {
            return _context.PaymentCharges.Where(c => c.Id == id).SingleOrDefault();
        }

        public override void Insert(IEnumerable<PaymentCharge> entities)
        {
            _context.PaymentCharges.AddRange(entities);
            _context.SaveChanges();
        }

        public IEnumerable<PaymentCharge> GetChargesByPatientId(int patientId)
        {
            return _context.PaymentCharges
                                  .Where(p => p.Payment.PatientId == patientId)
                                  .OrderByDescending(p => p.ChargeDate)
                                  .ToList();
        }

        public IEnumerable<PaymentCharge> GetChargesByLoginId(int loginId)
        {
            return _context.PaymentCharges
                .Where(p => p.LoginId == loginId)
                .OrderByDescending(p => p.ChargeDate)
                .ToList();
        }
    }
}
