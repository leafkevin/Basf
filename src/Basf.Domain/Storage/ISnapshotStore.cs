using System.Threading.Tasks;

namespace Basf.Domain.Storage
{
    public interface ISnapshotStore
    {
        void Create<TAggRoot, TAggRootId>(TAggRoot aggRoot) where TAggRoot : class, IAggregateRoot<TAggRootId>;
        Task CreateAsync<TAggRoot, TAggRootId>(TAggRoot aggRoot) where TAggRoot : class, IAggregateRoot<TAggRootId>;
        TAggRoot Get<TAggRoot, TAggRootId>(TAggRootId aggRootId) where TAggRoot : class, IAggregateRoot<TAggRootId>;
        Task<TAggRoot> GetAsync<TAggRoot, TAggRootId>(TAggRootId aggRootId) where TAggRoot : class, IAggregateRoot<TAggRootId>;
    }
}
