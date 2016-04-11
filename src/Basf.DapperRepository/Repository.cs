using Basf.Domain;
using Basf.Domain.Repository;
using System;
using System.Data;
using Basf.Orm;
using System.Threading.Tasks;

namespace Basf.DapperRepository
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        private IDbConnection Connection { get; set; }
        private IDbTransaction Transaction { get; set; }
        public Repository(IDbConnection connection, IDbTransaction transaction = null)
        {
            this.Connection = connection;
            this.Transaction = transaction;
        }
        public int Create(TEntity entity)
        {
            return this.Connection.Insert<TEntity>(entity, this.Transaction);
        }
        public TKey CreateSequence<TKey>(string strSequenceCode)
        {
            return default(TKey);
        }
        public int Delete(object objKey)
        {
            return this.Connection.Delete<TEntity>(objKey, this.Transaction);
        }
        public TEntity Get(object objKey)
        {
            return this.Connection.Get<TEntity>(objKey, this.Transaction);
        }
        public int Update(TEntity entity, object objKey)
        {
            return this.Connection.Update<TEntity>(entity, objKey, this.Transaction);
        }
        public Task<TKey> CreateSequenceAsync<TKey>(string strSequenceCode)
        {
            throw new NotImplementedException();
        }
        public Task<TEntity> GetAsync(object objKey)
        {
            return this.Connection.GetAsync<TEntity>(objKey, this.Transaction);
        }
        public Task<int> CreateAsync(TEntity entity)
        {
            return this.Connection.InsertAsync<TEntity>(entity, this.Transaction);
        }
        public Task<int> DeleteAsync(object objKey)
        {
            return this.Connection.DeleteAsync<TEntity>(objKey, this.Transaction);
        }
        public Task<int> UpdateAsync(TEntity entity, object objKey)
        {
            return this.Connection.UpdateAsync<TEntity>(objKey, this.Transaction);
        }
    }
    public abstract class Repository<TAggRoot, TAggRootId> : IRepository<TAggRoot, TAggRootId> where TAggRoot : class, IAggregateRoot<TAggRootId>
    {
        private IDbConnection Connection { get; set; }
        private IDbTransaction Transaction { get; set; }
        public Repository(IDbConnection connection, IDbTransaction transaction = null)
        {
            this.Connection = connection;
            this.Transaction = transaction;
        }
        public abstract TKey CreateSequence<TKey>(string strSequenceCode);
        public abstract TAggRoot Get(object objKey);
        public abstract int Create(TAggRoot entity);
        public abstract int Delete(object objKey);
        public abstract int Update(TAggRoot entity, object objKey);
        public abstract Task<TKey> CreateSequenceAsync<TKey>(string strSequenceCode);
        public abstract Task<TAggRoot> GetAsync(object objKey);
        public abstract Task<int> CreateAsync(TAggRoot entity);
        public abstract Task<int> DeleteAsync(object objKey);
        public abstract Task<int> UpdateAsync(TAggRoot entity, object objKey);
    }
}
