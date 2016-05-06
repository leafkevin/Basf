using System.Threading.Tasks;

namespace Basf.Domain.Storage
{
    public interface ISnapshotStore
    {
        void Create(IAggRoot aggRoot);
        Task CreateAsync(IAggRoot aggRoot);
        TAggRoot Get<TAggRoot, TAggRootId>(TAggRootId aggRootId) where TAggRoot : class, IAggRoot<TAggRootId>;
        Task<TAggRoot> GetAsync<TAggRoot, TAggRootId>(TAggRootId aggRootId) where TAggRoot : class, IAggRoot<TAggRootId>;
    }
}
