using System.Threading.Tasks;

namespace Basf.Domain
{
    public interface IDomainContext
    {
        Task Create<TEntity, TKey>(TKey id) where TEntity : IAggRoot<TKey>;
        Task<TEntity> Get<TEntity, TKey>(TKey id) where TEntity : IAggRoot<TKey>;
    }
}
