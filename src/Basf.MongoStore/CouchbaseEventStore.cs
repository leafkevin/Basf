using Basf.Domain.Event;
using System;
using System.Collections.Generic;

namespace Basf.MongoStore
{
    public class MongoEventStore : IEventStore
    {
        private IBucket bucket = null;
        public MongoEventStore()
        {
            Cluster cluster = new Cluster("couchbaseClients/couchbase");
            this.bucket = cluster.OpenBucket("EventStorage");
        }
        public void Add<TAggRootId>(params IDomainEvent<TAggRootId>[] domainEvents)
        {
            if (commands != null)
            {
                List<BsonDocument> documents = new List<BsonDocument>();
                foreach (ICommand command in commands)
                {
                    documents.Add(command.ToBsonDocument(command.GetType()));
                }
                return this.collection.InsertManyAsync(documents);
            }
            return Task.FromException(new Exception(""));

            if (domainEvents != null)
            {
                foreach (IDomainEvent<TAggRootId> domainEvent in domainEvents)
                {
                    this.bucket.Upsert<IDomainEvent<TAggRootId>>(domainEvent.UniqueId, domainEvent);
                }
            }
        }
        public void Add<TEvent, TAggRootId>(params TEvent[] domainEvents) where TEvent : IDomainEvent<TAggRootId>
        {
            if (domainEvents != null)
            {
                foreach (TEvent domainEvent in domainEvents)
                {
                    this.bucket.Upsert<TEvent>(domainEvent.UniqueId, domainEvent);
                }
            }
        }
        public IEnumerable<TAggRootId> Find<TAggRootId>(string commandId)
        {
            throw new NotImplementedException();
        }
        public IEnumerable<IDomainEvent<TAggRootId>> Find<TAggRootId>(Type aggRootType, TAggRootId aggRootId, string commandId)
        {
            throw new NotImplementedException();
        }
        public IEnumerable<IDomainEvent<TAggRootId>> Find<TAggRootId>(Type aggRootType, TAggRootId aggRootId, int startVersion)
        {
            throw new NotImplementedException();
        }
        public IEnumerable<TEvent> Find<TEvent, TAggRootId>(TAggRootId aggRootId, int startVersion) where TEvent : IDomainEvent<TAggRootId>
        {
            throw new NotImplementedException();
        }
    }
}
