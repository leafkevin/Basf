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
        public void Create(IAggRoot aggRoot)
        {
            var collection = this.db.GetCollection<IAggRoot>(aggRoot.GetType().Name);
            collection.ReplaceOne(f => f.UniqueId == aggRoot.UniqueId, aggRoot);
        }
        public async Task CreateAsync(IAggRoot aggRoot)
        {
            var collection = this.db.GetCollection<IAggRoot>(aggRoot.GetType().Name);
            await collection.ReplaceOneAsync(f => f.UniqueId == aggRoot.UniqueId, aggRoot);
        }
        public IAggRoot Get(string aggRootName, string aggRootId)
        {
            var collection = this.db.GetCollection<IAggRoot>(aggRootName);
            return collection.Find(f => f.UniqueId == aggRootId).FirstOrDefault();
        }
        public async Task<IAggRoot> GetAsync(string aggRootName, string aggRootId)
        {
            var collection = this.db.GetCollection<IAggRoot>(aggRootName);
            return await collection.Find(f => f.UniqueId == aggRootId).FirstOrDefaultAsync();
        }
    }
}
