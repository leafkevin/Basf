using System;
using System.Threading.Tasks;
using Basf.Domain.Command;
using Couchbase;
using Couchbase.Core;
using Basf.Domain.Storage;

namespace Basf.CouchbaseStore
{
    public class CouchbaseCommandStore : ICommandStore
    {
        private IBucket bucket = null;
        public CouchbaseCommandStore()
        {
            Cluster cluster = new Cluster("couchbaseClients/couchbase");
            this.bucket = cluster.OpenBucket("CommandStorage");
        }      
        public void Add<TCommand>(params TCommand[] commands) where TCommand : class, ICommand
        {
            if (commands != null)
            {
                foreach (TCommand command in commands)
                {
                    this.bucket.Upsert<TCommand>(command.UniqueId, command);
                }
            }
        }       
        public Task AddAsync<TCommand>(params TCommand[] commands) where TCommand : class, ICommand
        {
            if (commands != null)
            {
                return Task.Run(() =>
                {
                    foreach (TCommand command in commands)
                    {
                        this.bucket.Upsert<TCommand>(command.UniqueId, command);
                    }
                });
            }
            return Utility.NotNullAsync(commands, "commands");
        }
        public TCommand Get<TCommand>(string commandId) where TCommand :class, ICommand
        {
            return this.bucket.Get<TCommand>(commandId).Value;
        }
        public Task<TCommand> GetAsync<TCommand>(string commandId) where TCommand : class, ICommand
        {
            return Task.FromResult<TCommand>(this.bucket.Get<TCommand>(commandId).Value);
        }
    }
}
