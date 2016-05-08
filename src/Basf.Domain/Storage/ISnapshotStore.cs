using System.Threading.Tasks;

namespace Basf.Domain.Storage
{
    public interface ISnapshotStore
    {
        void Create(IAggRoot aggRoot);
        Task CreateAsync(IAggRoot aggRoot);
        IAggRoot Get(string aggRootName, string aggRootId);
        Task<IAggRoot> GetAsync(string aggRootName, string aggRootId);
    }
}
