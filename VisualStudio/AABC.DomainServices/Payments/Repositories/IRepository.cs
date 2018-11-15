using System.Collections.Generic;

namespace AABC.DomainServices.Payments.Repositories
{
    public interface IRepository<TEntity>
        where TEntity : class
    {
        TEntity Create();
        TEntity GetById(int id);
        void Insert(TEntity entity);
        void Insert(IEnumerable<TEntity> entities);
        void Save(TEntity entity);
    }
}
