using Basf.Domain.Storage;
using System.Threading.Tasks;
using Basf.Domain;
using MongoDB.Driver;

namespace Basf.MongoStore
{
    public class MongoSnapshotStore : ISnapshotStore
    {
        private IMongoDatabase db = null;
        public MongoSnapshotStore()
        {
            MongoClient client = new MongoClient(Utility.GetAppSettingValue("MongoStore"));
            this.db = client.GetDatabase("SnapshotStore");
        }
        public void Create<TAggRoot, TAggRootId>(TAggRoot aggRoot) where TAggRoot : class, IAggregateRoot<TAggRootId>
        {
            var collection = this.db.GetCollection<TAggRoot>(typeof(TAggRoot).Name);
            collection.DeleteOne(f => f.UniqueId.Equals(aggRoot.UniqueId));
            collection.InsertOne(aggRoot);
        }
        public Task CreateAsync<TAggRoot, TAggRootId>(TAggRoot aggRoot) where TAggRoot : class, IAggregateRoot<TAggRootId>
        {
            var collection = this.db.GetCollection<TAggRoot>(typeof(TAggRoot).Name);
            collection.DeleteOneAsync(f => f.UniqueId.Equals(aggRoot.UniqueId)).ContinueWith(t =>
            {
                return collection.InsertOneAsync(aggRoot);
            });
            return Utility.TaskDone;
        }
        public TAggRoot Get<TAggRoot, TAggRootId>(TAggRootId aggRootId) where TAggRoot : class, IAggregateRoot<TAggRootId>
        {
            var collection = this.db.GetCollection<TAggRoot>(typeof(TAggRoot).Name);
            return collection.Find(f => f.UniqueId.Equals(aggRootId)).FirstOrDefault();
        }
        public Task<TAggRoot> GetAsync<TAggRoot, TAggRootId>(TAggRootId aggRootId) where TAggRoot : class, IAggregateRoot<TAggRootId>
        {
            var collection = this.db.GetCollection<TAggRoot>(typeof(TAggRoot).Name);
            return collection.Find(f => f.UniqueId.Equals(aggRootId)).FirstOrDefaultAsync();
        }
    }
}
