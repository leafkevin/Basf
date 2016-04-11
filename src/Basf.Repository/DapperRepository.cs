using Basf.Domain;
using System;
using System.Data;
using Basf.Repository.Orm;
using System.Threading.Tasks;

namespace Basf.Repository
{
    public class DapperRepository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        private IDbConnection Connection { get; set; }
        private IDbTransaction Transaction { get; set; }
        public DapperRepository(IDbConnection connection, IDbTransaction transaction = null)
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
}
