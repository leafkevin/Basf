using System;
using System.Threading.Tasks;
using Basf.Domain.Command;
using MongoDB.Driver;
using System.Collections.Concurrent;
using Basf.Domain.Storage;
using System.Collections.Generic;
using MongoDB.Bson;

namespace Basf.MongoStore
{
    public class MongoCommandStore : ICommandStore
    {
        private ConcurrentDictionary<RuntimeTypeHandle, string> collectionMap = new ConcurrentDictionary<RuntimeTypeHandle, string>();
        private IMongoDatabase db = null;
        public MongoCommandStore()
        {
            MongoClient client = new MongoClient(Utility.GetAppSettingValue("MongoStore"));
            this.db = client.GetDatabase("CommandStore");
        }
        public CommandResult Add(params ICommand[] commands)
        {
            IMongoCollection<BsonDocument> collection = null;
            IMongoCollection<BsonDocument> resultCollection = null;
            try
            {
                if (commands != null)
                {
                    Type type = commands[0].GetType();
                    collection = this.db.GetCollection<BsonDocument>(type.Name);
                    if (commands.Length > 1)
                    {
                        List<BsonDocument> bsons = new List<BsonDocument>();
                        List<BsonDocument> resultBsons = new List<BsonDocument>();
                        foreach (ICommand command in commands)
                        {
                            bsons.Add(command.ToBsonDocument(type));
                            resultBsons.Add(this.GetResultBson(command, CommandResult.Stored));
                        }
                        collection.InsertMany(bsons);
                        resultCollection.InsertMany(resultBsons);
                    }
                    else
                    {
                        collection.InsertOne(commands[0].ToBsonDocument(type));
                        resultCollection.InsertOne(this.GetResultBson(commands[0], CommandResult.Stored));
                    }
                }
                return CommandResult.Stored;
            }
            catch (MongoWriteException ex)
            {
                if (ex.WriteError.Category == ServerErrorCategory.DuplicateKey)
                {
                    return this.GetResult(resultCollection, commands[0]);
                }
            }
            return CommandResult.Stored;
        }
        public async Task<CommandResult> AddAsync(params ICommand[] commands)
        {
            IMongoCollection<BsonDocument> collection = null;
            IMongoCollection<BsonDocument> resultCollection = null;
            try
            {
                if (commands != null)
                {
                    Type type = commands[0].GetType();
                    collection = this.db.GetCollection<BsonDocument>(type.Name);
                    if (commands.Length > 1)
                    {
                        List<BsonDocument> bsons = new List<BsonDocument>();
                        List<BsonDocument> resultBsons = new List<BsonDocument>();
                        foreach (ICommand command in commands)
                        {
                            bsons.Add(command.ToBsonDocument(type));
                            resultBsons.Add(this.GetResultBson(command, CommandResult.Stored));
                        }
                        await collection.InsertManyAsync(bsons);
                        await resultCollection.InsertManyAsync(resultBsons);
                    }
                    else
                    {
                        await collection.InsertOneAsync(commands[0].ToBsonDocument(type));
                        await resultCollection.InsertOneAsync(this.GetResultBson(commands[0], CommandResult.Stored));
                    }
                }
                return CommandResult.Stored;
            }
            catch (MongoWriteException ex)
            {
                if (ex.WriteError.Category == ServerErrorCategory.DuplicateKey)
                {
                    return await this.GetResultAsync(resultCollection, commands[0]);
                }
            }
            return CommandResult.Stored;
        }
        public TCommand Get<TCommand>(string commandId) where TCommand : class, ICommand
        {
            var collection = this.db.GetCollection<TCommand>(typeof(TCommand).Name);
            return collection.Find<TCommand>(f => f.UniqueId == commandId).FirstOrDefault();
        }
        public Task<TCommand> GetAsync<TCommand>(string commandId) where TCommand : class, ICommand
        {
            var collection = this.db.GetCollection<TCommand>(typeof(TCommand).Name);
            return collection.Find<TCommand>(f => f.UniqueId == commandId).FirstOrDefaultAsync();
        }
        public void UpdateResult(ICommand command, CommandResult result)
        {
            if (result < CommandResult.Executed)
            {
                return;
            }
            Type type = command.GetType();
            IMongoCollection<BsonDocument> collection = this.db.GetCollection<BsonDocument>(type.Name + "Result");
            var filter = Builders<BsonDocument>.Filter.Eq("UniqueId", command.UniqueId);
            var update = Builders<BsonDocument>.Update.Set("Result", result).Set("UpdateAt", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            collection.UpdateOne(filter, update);
        }
        public async Task UpdateResultAsync(ICommand command, CommandResult result)
        {
            if (result < CommandResult.Executed)
            {
                return;
            }
            Type type = command.GetType();
            IMongoCollection<BsonDocument> collection = this.db.GetCollection<BsonDocument>(type.Name + "Result");
            var filter = Builders<BsonDocument>.Filter.Eq("UniqueId", command.UniqueId);
            var update = Builders<BsonDocument>.Update.Set("Result", result).Set("UpdateAt", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            await collection.UpdateOneAsync(filter, update);
        }
        public List<TCommand> Find<TCommand>(CommandResult result) where TCommand : class, ICommand
        {
            //var collection = this.db.GetCollection<TCommand>(typeof(TCommand).Name);
            //return collection.Find<TCommand>(f => f.UniqueId.Equals(aggRootId) && f.Version > startVersion).ToList();
            return null;
        }
        public async Task<List<TCommand>> FindAsync<TCommand>(CommandResult result) where TCommand : class, ICommand
        {
            //var collection = this.db.GetCollection<TEvent>(typeof(TEvent).Name);
            //return await collection.Find<TEvent>(f => f.AggRootId.Equals(aggRootId) && f.Version > startVersion).ToListAsync();
            return null;
        }
        private CommandResult GetResult(IMongoCollection<BsonDocument> collection, ICommand command)
        {
            var filter = Builders<BsonDocument>.Filter.Eq("UniqueId", command.UniqueId);
            BsonDocument result = collection.Find(filter).FirstOrDefault();
            return (CommandResult)result["ActionResult"].ToInt32();
        }
        private async Task<CommandResult> GetResultAsync(IMongoCollection<BsonDocument> collection, ICommand command)
        {
            var filter = Builders<BsonDocument>.Filter.Eq("UniqueId", command.UniqueId);
            BsonDocument result = await collection.Find(filter).FirstOrDefaultAsync();
            return (CommandResult)result["Result"].ToInt32();
        }
        private BsonDocument GetResultBson(ICommand command, CommandResult result)
        {
            return new BsonDocument
            {
                {"CommandId",command.UniqueId},{"Result",result},
                {"CreateAt",DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")},{"UpdateAt",DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}
            };
        }
    }
}
