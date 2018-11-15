using AABC.Data.V2;
using AABC.Domain2.Payments;
using System.Collections.Generic;
using System.Linq;

namespace AABC.DomainServices.Payments.Repositories
{
    public class CreditCardRepository : BaseRepository<CreditCard>
    {
        public CreditCardRepository(CoreContext context) : base(context)
        {
        }

        public override CreditCard Create()
        {
            return _context.CreditCards.Create();
        }

        public override CreditCard GetById(int id)
        {
            return _context.CreditCards.Where(c => c.Id == id).SingleOrDefault();
        }

        public override void Insert(IEnumerable<CreditCard> entities)
        {
            _context.CreditCards.AddRange(entities);
            _context.SaveChanges();
        }
    }
}
