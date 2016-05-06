using Basf.Domain.Event;
using Basf.Domain.Storage;
using MongoDB.Bson;
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
            MongoClient client = new MongoClient(Utility.GetAppSettingValue("MongoStore"));
            this.db = client.GetDatabase("EventStore");
        }
        public EventResult Add(params IDomainEvent[] domainEvents)
        {
            IMongoCollection<BsonDocument> collection = null;
            IMongoCollection<BsonDocument> resultCollection = null;
            try
            {
                if (domainEvents != null)
                {
                    Type type = domainEvents[0].GetType();
                    collection = this.db.GetCollection<BsonDocument>(type.Name);
                    if (domainEvents.Length > 1)
                    {
                        List<BsonDocument> bsons = new List<BsonDocument>();
                        List<BsonDocument> resultBsons = new List<BsonDocument>();
                        foreach (IDomainEvent domainEvent in domainEvents)
                        {
                            bsons.Add(domainEvent.ToBsonDocument(type));
                            resultBsons.Add(this.GetResultBson(domainEvent, EventResult.Stored));
                        }
                        collection.InsertMany(bsons);
                        resultCollection.InsertMany(resultBsons);
                    }
                    else
                    {
                        collection.InsertOneAsync(domainEvents[0].ToBsonDocument(type));
                        resultCollection.InsertOne(this.GetResultBson(domainEvents[0], EventResult.Stored));
                    }
                }
                return EventResult.Stored;
            }
            catch (MongoWriteException ex)
            {
                if (ex.WriteError.Category == ServerErrorCategory.DuplicateKey)
                {
                    return this.GetResult(resultCollection, domainEvents[0]);
                }
                throw ex;
            }
            return EventResult.Stored;
        }
        public async Task<EventResult> AddAsync(params IDomainEvent[] domainEvents)
        {
            IMongoCollection<BsonDocument> collection = null;
            IMongoCollection<BsonDocument> resultCollection = null;
            try
            {
                if (domainEvents != null)
                {
                    Type type = domainEvents[0].GetType();
                    collection = this.db.GetCollection<BsonDocument>(type.Name);
                    if (domainEvents.Length > 1)
                    {
                        List<BsonDocument> bsons = new List<BsonDocument>();
                        List<BsonDocument> resultBsons = new List<BsonDocument>();
                        foreach (IDomainEvent domainEvent in domainEvents)
                        {
                            bsons.Add(domainEvent.ToBsonDocument(type));
                            resultBsons.Add(this.GetResultBson(domainEvent, EventResult.Stored));
                        }
                        await collection.InsertManyAsync(bsons);
                        await resultCollection.InsertManyAsync(resultBsons);
                    }
                    else
                    {
                        await collection.InsertOneAsync(domainEvents[0].ToBsonDocument(type));
                        await resultCollection.InsertOneAsync(this.GetResultBson(domainEvents[0], EventResult.Stored));
                    }
                }
                return EventResult.Stored;
            }
            catch (MongoWriteException ex)
            {
                if (ex.WriteError.Category == ServerErrorCategory.DuplicateKey)
                {
                    return await this.GetResultAsync(resultCollection, domainEvents[0]);
                }
                throw ex;
            }
            return EventResult.Stored;
        }
        public TEvent Get<TEvent, TAggRootId>(TAggRootId aggRootId, int version) where TEvent : class, IDomainEvent<TAggRootId>
        {
            var collection = this.db.GetCollection<TEvent>(typeof(TEvent).Name);
            return collection.Find<TEvent>(f => f.AggRootId.Equals(aggRootId) && f.Version == version).FirstOrDefault();
        }
        public async Task<TEvent> GetAsync<TEvent, TAggRootId>(TAggRootId aggRootId, int version) where TEvent : class, IDomainEvent<TAggRootId>
        {
            var collection = this.db.GetCollection<TEvent>(typeof(TEvent).Name);
            return await collection.Find<TEvent>(f => f.AggRootId.Equals(aggRootId) && f.Version == version).FirstOrDefaultAsync();
        }
        public List<TEvent> Find<TEvent, TAggRootId>(TAggRootId aggRootId, int startVersion) where TEvent : class, IDomainEvent<TAggRootId>
        {
            var collection = this.db.GetCollection<TEvent>(typeof(TEvent).Name);
            return collection.Find<TEvent>(f => f.AggRootId.Equals(aggRootId) && f.Version > startVersion).ToList();
        }
        public async Task<List<TEvent>> FindAsync<TEvent, TAggRootId>(TAggRootId aggRootId, int startVersion) where TEvent : class, IDomainEvent<TAggRootId>
        {
            var collection = this.db.GetCollection<TEvent>(typeof(TEvent).Name);
            return await collection.Find<TEvent>(f => f.AggRootId.Equals(aggRootId) && f.Version > startVersion).ToListAsync();
        }
        public void UpdateResult(IDomainEvent domainEvent, EventResult result)
        {
            if (result < EventResult.Executed)
            {
                return;
            }
            Type type = domainEvent.GetType();
            IMongoCollection<BsonDocument> collection = this.db.GetCollection<BsonDocument>(type.Name + "Result");
            var builder = Builders<BsonDocument>.Filter;
            var filter = builder.Eq("UniqueId", domainEvent.AggRootId) & builder.Eq("Version", domainEvent.Version) & builder.Lt("Result", result);
            var update = Builders<BsonDocument>.Update.Set("Result", result).Set("UpdateAt", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            collection.UpdateOne(filter, update);
        }
        public async Task UpdateResultAsync(IDomainEvent domainEvent, EventResult result)
        {
            if (result < EventResult.Executed)
            {
                return;
            }
            Type type = typeof(IDomainEvent);
            IMongoCollection<BsonDocument> collection = this.db.GetCollection<BsonDocument>(type.Name + "Result");
            var builder = Builders<BsonDocument>.Filter;
            var filter = builder.Eq("UniqueId", domainEvent.AggRootId) & builder.Eq("Version", domainEvent.Version) & builder.Lt("Result", result);
            var update = Builders<BsonDocument>.Update.Set("Result", result).Set("UpdateAt", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            await collection.UpdateOneAsync(filter, update);
        }
        private EventResult GetResult(IMongoCollection<BsonDocument> collection, IDomainEvent domainEvent)
        {
            var builder = Builders<BsonDocument>.Filter;
            var filter = builder.Eq("UniqueId", domainEvent.AggRootId) & builder.Eq("Version", domainEvent.Version);
            BsonDocument result = collection.Find(filter).FirstOrDefault();
            return (EventResult)result["ActionResult"].ToInt32();
        }
        private async Task<EventResult> GetResultAsync(IMongoCollection<BsonDocument> collection, IDomainEvent domainEvent)
        {
            var builder = Builders<BsonDocument>.Filter;
            var filter = builder.Eq("UniqueId", domainEvent.AggRootId) & builder.Eq("Version", domainEvent.Version);
            BsonDocument result = await collection.Find(filter).FirstOrDefaultAsync();
            return (EventResult)result["Result"].ToInt32();
        }
        private BsonDocument GetResultBson(IDomainEvent domainEvent, EventResult result)
        {
            return new BsonDocument
            {
                {"AggRootId",domainEvent.AggRootId},{"Version",domainEvent.Version},{"Result",result},
                {"CreateAt",DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")},{"UpdateAt",DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}
            };
        }
    }
}
