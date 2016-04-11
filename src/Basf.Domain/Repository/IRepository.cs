using System.Threading.Tasks;

namespace Basf.Domain.Repository
{
    public interface IRepository<TEntity> where TEntity : class
    {
        TKey CreateSequence<TKey>(string strSequenceCode);
        TEntity Get(object objKey);
        int Create(TEntity entity);
        int Delete(object objKey);
        int Update(TEntity entity, object objKey);
        Task<TKey> CreateSequenceAsync<TKey>(string strSequenceCode);
        Task<TEntity> GetAsync(object objKey);
        Task<int> CreateAsync(TEntity entity);
        Task<int> DeleteAsync(object objKey);
        Task<int> UpdateAsync(TEntity entity, object objKey);
    }
    public interface IRepository<TAggRoot, TAggRootId> : IRepository<TAggRoot> where TAggRoot : class, IAggregateRoot<TAggRootId>
    {
    }
}
