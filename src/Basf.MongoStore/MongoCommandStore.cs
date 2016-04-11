using System;
using System.Threading.Tasks;
using Basf.Domain.Command;
using MongoDB.Driver;
using System.Collections.Concurrent;
using Basf.Domain.Storage;

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
        public void Add<TCommand>(params TCommand[] commands) where TCommand : class, ICommand
        {
            if (commands != null)
            {
                string commandType = null;
                if (this.collectionMap.TryGetValue(typeof(TCommand).TypeHandle, out commandType))
                {
                    var collection = this.db.GetCollection<TCommand>(commandType);
                    collection.InsertMany(commands);
                }
                Utility.Fail("没有映射{0}类型Bson序列化配置", typeof(TCommand).FullName);
            }
        }
        public Task AddAsync<TCommand>(params TCommand[] commands) where TCommand : class, ICommand
        {
            if (commands != null)
            {
                var collection = this.db.GetCollection<TCommand>(typeof(TCommand).Name);
                return collection.InsertManyAsync(commands);
            }
            return Utility.NotNullAsync(commands, "commands");
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
    }
}
