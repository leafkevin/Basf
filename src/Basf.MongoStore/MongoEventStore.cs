using Basf.Data;
using Basf.Domain.Event;
using Basf.Domain.Storage;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Basf.MongoStore
{
    public class MongoEventStore : IEventStore
    {
        private IMongoDatabase db = null;
        public MongoEventStore()
        {
            MongoClient client = new MongoClient(Utility.GetAppSettingValue("MongoStore", ""));
            this.db = client.GetDatabase("EventStore");
        }
        public ActionResponse<EventResult> Add(IDomainEvent domainEvent)
        {
            IMongoCollection<IDomainEvent> collection = null;
            IMongoCollection<EventStoreResult> resultCollection = null;
            try
            {
                EventStoreResult result = new EventStoreResult(domainEvent);
                int index = domainEvent.AggRootType.LastIndexOf(".");
                string aggRootName = domainEvent.AggRootType.Substring(index);
                collection = this.db.GetCollection<IDomainEvent>(aggRootName);
                resultCollection = this.db.GetCollection<EventStoreResult>(aggRootName + "Result");
                collection.InsertOne(domainEvent);
                resultCollection.InsertOne(result);
                return ActionResponse.Succeed<EventResult>(EventResult.Stored);
            }
            catch (MongoWriteException ex)
            {
                if (ex.WriteError.Category == ServerErrorCategory.DuplicateKey)
                {
                    return this.GetResult(resultCollection, domainEvent);
                }
                return ActionResponse.Fail<EventResult>(1, ex.ToString());
            }
        }
        public async Task<ActionResponse<EventResult>> AddAsync(IDomainEvent domainEvent)
        {
            IMongoCollection<IDomainEvent> collection = null;
            IMongoCollection<EventStoreResult> resultCollection = null;
            try
            {
                EventStoreResult result = new EventStoreResult(domainEvent);
                int index = domainEvent.AggRootType.LastIndexOf(".");
                string aggRootName = domainEvent.AggRootType.Substring(index);
                collection = this.db.GetCollection<IDomainEvent>(aggRootName);
                resultCollection = this.db.GetCollection<EventStoreResult>(aggRootName + "Result");
                await collection.InsertOneAsync(domainEvent);
                await resultCollection.InsertOneAsync(result);
                return ActionResponse.Succeed<EventResult>(EventResult.Stored);
            }
            catch (MongoWriteException ex)
            {
                if (ex.WriteError.Category == ServerErrorCategory.DuplicateKey)
                {
                    return await this.GetResultAsync(resultCollection, domainEvent);
                }
                return ActionResponse.Fail<EventResult>(1, ex.ToString());
            }
        }
        public IDomainEvent Get(string aggRootTypeName, string aggRootId, int version)
        {
            var collection = this.db.GetCollection<IDomainEvent>(aggRootTypeName);
            return collection.Find(f => f.AggRootId == aggRootId && f.Version == version).FirstOrDefault();
        }
        public async Task<IDomainEvent> GetAsync(string aggRootTypeName, string aggRootId, int version)
        {
            var collection = this.db.GetCollection<IDomainEvent>(aggRootTypeName);
            return await collection.Find(f => f.AggRootId == aggRootId && f.Version == version).FirstOrDefaultAsync();
        }
        public List<IDomainEvent> Find(string aggRootTypeName, string aggRootId, int startVersion)
        {
            var collection = this.db.GetCollection<IDomainEvent>(aggRootTypeName);
            return collection.Find(f => f.AggRootId == aggRootId && f.Version >= startVersion).ToList();
        }
        public async Task<List<IDomainEvent>> FindAsync(string aggRootTypeName, string aggRootId, int startVersion)
        {
            var collection = this.db.GetCollection<IDomainEvent>(aggRootTypeName);
            return await collection.Find(f => f.AggRootId == aggRootId && f.Version >= startVersion).ToListAsync();
        }
        public void UpdateResult(IDomainEvent domainEvent, EventResult result)
        {
            if (result < EventResult.Executed)
            {
                return;
            }
            Type type = domainEvent.GetType();
            var collection = this.db.GetCollection<EventStoreResult>(type.Name + "Result");
            var update = Builders<EventStoreResult>.Update.Set(f => f.Result, result).
                Set(f => f.UpdateAt, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            collection.UpdateOne(f => f.AggRootType == domainEvent.AggRootType &&
                f.AggRootId == domainEvent.AggRootId && f.Version == domainEvent.Version, update);
        }
        public async Task UpdateResultAsync(IDomainEvent domainEvent, EventResult result)
        {
            if (result < EventResult.Executed)
            {
                return;
            }
            Type type = domainEvent.GetType();
            var collection = this.db.GetCollection<EventStoreResult>(type.Name + "Result");
            var update = Builders<EventStoreResult>.Update.Set(f => f.Result, result).
                Set(f => f.UpdateAt, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            await collection.UpdateOneAsync(f => f.AggRootType == domainEvent.AggRootType &&
                 f.AggRootId == domainEvent.AggRootId && f.Version == domainEvent.Version, update);
        }
        private ActionResponse<EventResult> GetResult(IMongoCollection<EventStoreResult> collection, IDomainEvent domainEvent)
        {
            EventStoreResult result = collection.Find(f => f.AggRootType == domainEvent.AggRootType &&
               f.AggRootId == domainEvent.AggRootId && f.Version == domainEvent.Version).FirstOrDefault();
            return ActionResponse.Succeed<EventResult>(result.Result);
        }
        private async Task<ActionResponse<EventResult>> GetResultAsync(IMongoCollection<EventStoreResult> collection, IDomainEvent domainEvent)
        {
            EventStoreResult result = await collection.Find(f => f.AggRootType == domainEvent.AggRootType &&
               f.AggRootId == domainEvent.AggRootId && f.Version == domainEvent.Version).FirstOrDefaultAsync();
            return ActionResponse.Succeed<EventResult>(result.Result);
        }
    }
}
