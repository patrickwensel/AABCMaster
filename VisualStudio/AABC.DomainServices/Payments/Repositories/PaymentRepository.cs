using AABC.Data.V2;
using AABC.Domain2.Payments;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AABC.DomainServices.Payments.Repositories
{
    public class PaymentRepository : BaseRepository<Payment>, IPaymentRepository
    {
        public PaymentRepository(CoreContext context) : base(context)
        {
        }

        public override Payment Create()
        {
            return _context.Payments.Create();
        }

        public override Payment GetById(int id)
        {
            return _context.Payments.SingleOrDefault(m => m.Id == id);
        }

        public override void Insert(IEnumerable<Payment> payments)
        {
            _context.Payments.AddRange(payments);
            _context.SaveChanges();
        }

        public IEnumerable<Payment> GetScheduledPaymentsByLoginId(int loginId, DateTime date)
        {
            return _context.Payments
                        .Where(p => p.LoginId == loginId && p.PaymentSchedules.Any(m => m.ScheduledDate >= date))
                        .OrderByDescending(p => p.Active)
                        .ToList();
        }

        public IEnumerable<Payment> GetScheduledPaymentsByPatientId(int patientId, DateTime date) //PaymentWithScheduledVM
        {
            return _context.Payments
                        .Where(p => p.PatientId == patientId && p.PaymentSchedules.Any(m => m.ScheduledDate >= date))
                        .OrderByDescending(p => p.Active)
                        .ToList();
        }
    }
}
