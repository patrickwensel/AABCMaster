using AABC.Data.V2;
using System.Collections.Generic;

namespace AABC.DomainServices.Payments.Repositories
{
    public abstract class BaseRepository<TEntity> : IRepository<TEntity>
        where TEntity : class
    {
        protected readonly CoreContext _context;

        public BaseRepository(CoreContext context)
        {
            _context = context;
        }

        public abstract TEntity Create();
        public abstract TEntity GetById(int id);
        public abstract void Insert(IEnumerable<TEntity> entities);


        public void Insert(TEntity entity)
        {
            Insert(new TEntity[] { entity });
        }

        public void Save(TEntity entity)
        {
            _context.SaveChanges();
        }
    }
}
