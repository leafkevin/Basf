using System;
using System.Threading.Tasks;
using Basf.Domain.Command;
using MongoDB.Driver;
using System.Collections.Concurrent;
using Basf.Domain.Storage;
using System.Collections.Generic;
using Basf.Data;
using System.Linq;

namespace Basf.MongoStore
{
    public class MongoCommandStore : ICommandStore
    {
        private ConcurrentDictionary<RuntimeTypeHandle, string> collectionMap = new ConcurrentDictionary<RuntimeTypeHandle, string>();
        private IMongoDatabase db = null;
        public MongoCommandStore()
        {
            MongoClient client = new MongoClient(Utility.GetAppSettingValue("MongoStore", ""));
            this.db = client.GetDatabase("CommandStore");
        }
        public ActionResponse<CommandResult> Add(ICommand command)
        {
            IMongoCollection<ICommand> collection = null;
            IMongoCollection<CommandStoreResult> resultCollection = null;
            try
            {
                Type type = command.GetType();
                CommandStoreResult result = new CommandStoreResult(command);
                collection = this.db.GetCollection<ICommand>(type.Name);
                resultCollection = this.db.GetCollection<CommandStoreResult>(type.Name + "Result");
                collection.InsertOne(command);
                resultCollection.InsertOne(result);
                return ActionResponse.Succeed<CommandResult>(CommandResult.Stored);
            }
            catch (MongoWriteException ex)
            {
                if (ex.WriteError.Category == ServerErrorCategory.DuplicateKey)
                {
                    return this.GetResult(resultCollection, command);
                }
                return ActionResponse.Fail<CommandResult>(1, ex.ToString());
            }
        }
        public async Task<ActionResponse<CommandResult>> AddAsync(ICommand command)
        {
            IMongoCollection<ICommand> collection = null;
            IMongoCollection<CommandStoreResult> resultCollection = null;
            try
            {
                Type type = command.GetType();
                CommandStoreResult result = new CommandStoreResult(command);
                collection = this.db.GetCollection<ICommand>(type.Name);
                resultCollection = this.db.GetCollection<CommandStoreResult>(type.Name + "Result");
                await collection.InsertOneAsync(command);
                await resultCollection.InsertOneAsync(result);
                return ActionResponse.Succeed<CommandResult>(CommandResult.Stored);
            }
            catch (MongoWriteException ex)
            {
                if (ex.WriteError.Category == ServerErrorCategory.DuplicateKey)
                {
                    return await this.GetResultAsync(resultCollection, command);
                }
                return ActionResponse.Fail<CommandResult>(1, ex.ToString());
            }
        }
        public ICommand Get(string commandTypeName, string commandId)
        {
            var collection = this.db.GetCollection<ICommand>(commandTypeName);
            return collection.Find(f => f.UniqueId == commandId).FirstOrDefault();
        }
        public async Task<ICommand> GetAsync(string commandTypeName, string commandId)
        {
            var collection = this.db.GetCollection<ICommand>(commandTypeName);
            return await collection.Find(f => f.UniqueId == commandId).FirstOrDefaultAsync();
        }
        public List<ICommand> Find(string commandTypeName, CommandResult result)
        {
            var resultCollection = this.db.GetCollection<CommandStoreResult>(commandTypeName + "Result");
            var resultList = resultCollection.Find(f => f.Result == result).ToList().Select(f => f.CommandId).ToList();
            var collection = this.db.GetCollection<ICommand>(commandTypeName);
            return collection.Find(f => resultList.Contains(f.UniqueId)).ToList();
        }
        public async Task<List<ICommand>> FindAsync(string commandTypeName, CommandResult result)
        {
            var resultCollection = this.db.GetCollection<CommandStoreResult>(commandTypeName + "Result");
            var list = await resultCollection.Find(f => f.Result == result).ToListAsync();
            var resultList = list.Select(f => f.CommandId).ToList();
            var collection = this.db.GetCollection<ICommand>(commandTypeName);
            return await collection.Find(f => resultList.Contains(f.UniqueId)).ToListAsync();
        }
        public void UpdateResult(ICommand command, CommandResult result)
        {
            if (result < CommandResult.Executed)
            {
                return;
            }
            Type type = command.GetType();
            var collection = this.db.GetCollection<CommandStoreResult>(type.Name + "Result");
            var update = Builders<CommandStoreResult>.Update.Set(f => f.Result, result).
                Set(f => f.UpdateAt, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            collection.UpdateOne(f => f.CommandId == command.UniqueId, update);
        }
        public async Task UpdateResultAsync(ICommand command, CommandResult result)
        {
            if (result < CommandResult.Executed)
            {
                return;
            }
            Type type = command.GetType();
            var collection = this.db.GetCollection<CommandStoreResult>(type.Name + "Result");
            var update = Builders<CommandStoreResult>.Update.Set(f => f.Result, result).
                Set(f => f.UpdateAt, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            await collection.UpdateOneAsync(f => f.CommandId == command.UniqueId, update);
        }
        private ActionResponse<CommandResult> GetResult(IMongoCollection<CommandStoreResult> collection, ICommand command)
        {
            var result = collection.Find(f => f.CommandId == command.UniqueId).FirstOrDefault();
            return ActionResponse.Succeed<CommandResult>(result.Result);
        }
        private async Task<ActionResponse<CommandResult>> GetResultAsync(IMongoCollection<CommandStoreResult> collection, ICommand command)
        {
            var result = await collection.Find(f => f.CommandId == command.UniqueId).FirstOrDefaultAsync();
            return ActionResponse.Succeed<CommandResult>(result.Result);
        }
    }
}
