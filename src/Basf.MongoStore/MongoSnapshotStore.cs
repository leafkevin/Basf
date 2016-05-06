using Basf.Domain.Storage;
using System.Threading.Tasks;
using Basf.Domain;
using MongoDB.Driver;
using MongoDB.Bson;

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
        public void Create(IAggRoot aggRoot)
        {
            var collection = this.db.GetCollection<BsonDocument>(aggRoot.GetType().Name);
            var filter = Builders<BsonDocument>.Filter.Eq("UniqueId", aggRoot.UniqueId);
            collection.ReplaceOne(filter, aggRoot.ToBsonDocument());
        }
        public async Task CreateAsync(IAggRoot aggRoot)
        {
            var collection = this.db.GetCollection<BsonDocument>(aggRoot.GetType().Name);
            var filter = Builders<BsonDocument>.Filter.Eq("UniqueId", aggRoot.UniqueId);
            await collection.ReplaceOneAsync(filter, aggRoot.ToBsonDocument());
        }
        public TAggRoot Get<TAggRoot, TAggRootId>(TAggRootId aggRootId) where TAggRoot : class, IAggRoot<TAggRootId>
        {
            var collection = this.db.GetCollection<TAggRoot>(typeof(TAggRoot).Name);
            return collection.Find(f => f.UniqueId.Equals(aggRootId)).FirstOrDefault();
        }
        public async Task<TAggRoot> GetAsync<TAggRoot, TAggRootId>(TAggRootId aggRootId) where TAggRoot : class, IAggRoot<TAggRootId>
        {
            var collection = this.db.GetCollection<TAggRoot>(typeof(TAggRoot).Name);
            return await collection.Find(f => f.UniqueId.Equals(aggRootId)).FirstOrDefaultAsync();
        }
    }
}
