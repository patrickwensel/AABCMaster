using AABC.Data.V2;
using AABC.Domain2.Payments;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AABC.DomainServices.Payments.Repositories
{
    public class PaymentScheduleRepository : BaseRepository<PaymentSchedule>, IPaymentScheduleRepository
    {
        public PaymentScheduleRepository(CoreContext context) : base(context)
        {
        }

        public override PaymentSchedule Create()
        {
            return _context.PaymentSchedules.Create();
        }

        public override PaymentSchedule GetById(int id)
        {
            return _context.PaymentSchedules.Where(c => c.Id == id).SingleOrDefault();
        }

        public IEnumerable<PaymentSchedule> GetUnpaidSchedules(DateTime date)
        {
            return _context.PaymentSchedules.Where(p =>
                p.Payment.Active == true && 
                p.Payment.PaymentType == PaymentType.recurring && 
                p.Payment.RecurringDateStart <= date && p.Payment.RecurringDateEnd >= date && 
                p.Payment.CreditCardId.HasValue &&
                p.ScheduledDate <= date && 
                p.PaymentChargeId == null).ToList();
        }

        public override void Insert(IEnumerable<PaymentSchedule> entities)
        {
            _context.PaymentSchedules.AddRange(entities);
            _context.SaveChanges();
        }
    }
}
