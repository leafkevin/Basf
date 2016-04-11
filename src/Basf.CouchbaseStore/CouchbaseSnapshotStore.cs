using System;
using System.Threading.Tasks;
using Basf.Domain.Storage;

namespace Basf.CouchbaseStore
{
    public class CouchbaseSnapshotStore : ISnapshotStore
    {
        void ISnapshotStore.Create<TAggRoot, TAggRootId>(TAggRoot aggRoot)
        {
            throw new NotImplementedException();
        }

        Task ISnapshotStore.CreateAsync<TAggRoot, TAggRootId>(TAggRoot aggRoot)
        {
            throw new NotImplementedException();
        }

        TAggRoot ISnapshotStore.Get<TAggRoot, TAggRootId>(TAggRootId aggRootId)
        {
            throw new NotImplementedException();
        }

        Task<TAggRoot> ISnapshotStore.GetAsync<TAggRoot, TAggRootId>(TAggRootId aggRootId)
        {
            throw new NotImplementedException();
        }
    }
}
