using Basf.Domain.Event;
using System;
using System.Collections.Generic;
using Couchbase.Core;
using Couchbase;
using System.Threading.Tasks;
using Basf.Domain.Storage;

namespace Basf.CouchbaseStore
{
    public class CouchbaseEventStore : IEventStore
    {
        private IBucket bucket = null;
        public CouchbaseEventStore()
        {
            Cluster cluster = new Cluster("couchbaseClients/couchbase");
            this.bucket = cluster.OpenBucket("EventStorage");
        }
        public void Add<TAggRootId>(params IDomainEvent<TAggRootId>[] domainEvents)
        {
            if (domainEvents != null)
            {
                foreach (IDomainEvent<TAggRootId> domainEvent in domainEvents)
                {
                    this.bucket.Upsert<IDomainEvent<TAggRootId>>(domainEvent.UniqueId, domainEvent);
                }
            }
        }
        public void Add<TEvent, TAggRootId>(params TEvent[] domainEvents) where TEvent : class, IDomainEvent<TAggRootId>
        {
            if (domainEvents != null)
            {
                foreach (TEvent domainEvent in domainEvents)
                {
                    this.bucket.Upsert<TEvent>(domainEvent.UniqueId, domainEvent);
                }
            }
        }
        public Task AddAsync<TEvent, TAggRootId>(params TEvent[] domainEvents) where TEvent : class, IDomainEvent<TAggRootId>
        {
            throw new NotImplementedException();
        }
        public List<TEvent> Find<TEvent, TAggRootId>(TAggRootId aggRootId, int startVersion) where TEvent : class, IDomainEvent<TAggRootId>
        {
            throw new NotImplementedException();
        }
        public Task<List<TEvent>> FindAsync<TEvent, TAggRootId>(TAggRootId aggRootId, int startVersion) where TEvent : class, IDomainEvent<TAggRootId>
        {
            throw new NotImplementedException();
        }
    }
}
