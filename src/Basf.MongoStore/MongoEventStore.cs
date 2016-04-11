using Basf.Domain.Event;
using Basf.Domain.Storage;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Basf.MongoStore
{
    public class MongoEventStore : IEventStore
    {
        private IMongoDatabase db = null;
        public MongoEventStore()
        {
            MongoClient client = new MongoClient(Utility.GetAppSettingValue("MongoStore"));
            this.db = client.GetDatabase("EventStore");
        }
        public void Add<TEvent, TAggRootId>(params TEvent[] domainEvents) where TEvent : class, IDomainEvent<TAggRootId>
        {
            if (domainEvents != null)
            {
                var collection = this.db.GetCollection<TEvent>(typeof(TEvent).Name);
                collection.InsertMany(domainEvents);
            }
        }
        public Task AddAsync<TEvent, TAggRootId>(params TEvent[] domainEvents) where TEvent : class, IDomainEvent<TAggRootId>
        {
            if (domainEvents != null)
            {
                var collection = this.db.GetCollection<TEvent>(typeof(TEvent).Name);
                return collection.InsertManyAsync(domainEvents);
            }
            return Utility.NotNullAsync(domainEvents, "domainEvents");
        }
        public List<TEvent> Find<TEvent, TAggRootId>(TAggRootId aggRootId, int startVersion) where TEvent : class, IDomainEvent<TAggRootId>
        {
            var collection = this.db.GetCollection<TEvent>(typeof(TEvent).Name);
            return collection.Find<TEvent>(f => f.AggRootId.Equals(aggRootId) && f.Version > startVersion).ToList();
        }
        public Task<List<TEvent>> FindAsync<TEvent, TAggRootId>(TAggRootId aggRootId, int startVersion) where TEvent : class, IDomainEvent<TAggRootId>
        {
            var collection = this.db.GetCollection<TEvent>(typeof(TEvent).Name);
            collection.FindAsync<TEvent>(f => f.AggRootId.Equals(aggRootId) && f.Version > startVersion).ContinueWith(t =>
            {
                return t.Result.ToListAsync();
            });
            return Task.FromResult<List<TEvent>>(null);
        }
    }
}
