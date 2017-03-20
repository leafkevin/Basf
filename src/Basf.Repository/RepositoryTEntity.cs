using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Basf.Repository
{
    public partial class Repository<TEntity> : IRepository<TEntity> where TEntity : class, new()
    {
        protected DbConnection connection = null;
        protected DbTransaction Transaction { get; set; }
        protected static EntityMapper Mapper { get; private set; } = new EntityMapper(typeof(TEntity));
        public string ConnString { get; private set; }
        public IOrmProvider Provider { get; private set; }
        public Repository()
        {
            this.ConnString = OrmProviderFactory.DefaultConnString;
            this.Provider = OrmProviderFactory.DefaultProvider;
        }
        public Repository(string connString)
        {
            this.ConnString = connString;
            this.Provider = OrmProviderFactory.GetProvider(connString);
        }
        public Repository(string connString, DbTransaction transaction)
        {
            this.ConnString = connString;
            this.Provider = OrmProviderFactory.GetProvider(connString);
            if (transaction != null)
            {
                this.Transaction = transaction;
                this.connection = transaction.Connection;
            }
        }
        public TEntity Get(TEntity key)
        {
            var sql = GetSqlCache(this.ConnString, "GET", this.Provider);
            return this.QueryFirstImpl<TEntity>(Mapper.EntityType, sql, CommandType.Text, key, true);
        }
        public int Create(TEntity entity)
        {
            var sql = GetSqlCache(this.ConnString, "CREATE", this.Provider);
            return this.ExecSqlImpl(sql, CommandType.Text, entity);
        }
        public int Delete(TEntity key)
        {
            var sql = GetSqlCache(this.ConnString, "DELETE", this.Provider);
            return this.ExecSqlImpl(sql, CommandType.Text, key, true);
        }
        public int Update(string sql, TEntity entity = null)
        {
            return this.ExecSqlImpl(sql, CommandType.Text, entity);
        }
        public int Update<TFields>(Expression<Func<TEntity, TFields>> fieldsExpression, TEntity objParameters = null)
        {
            var sql = GetUpdateFieldsSql(fieldsExpression, this.Provider);
            return this.ExecSqlImpl(sql, CommandType.Text, objParameters);
        }
        public TEntity QueryFirst(string sql, TEntity objParameters = null, CommandType cmdType = CommandType.Text)
        {
            return this.QueryFirstImpl<TEntity>(Mapper.EntityType, sql, cmdType, objParameters);
        }
        public TTarget QueryFirst<TTarget>(string sql, TEntity objParameters = null, CommandType cmdType = CommandType.Text)
        {
            return this.QueryFirstImpl<TTarget>(typeof(TTarget), sql, cmdType, objParameters);
        }
        public List<TEntity> Query(string sql, TEntity objParameters = null, CommandType cmdType = CommandType.Text)
        {
            return this.QueryImpl<TEntity>(Mapper.EntityType, sql, cmdType, objParameters);
        }
        public List<TTarget> Query<TTarget>(string sql, TEntity objParameters = null, CommandType cmdType = CommandType.Text)
        {
            return this.QueryImpl<TTarget>(typeof(TTarget), sql, cmdType, objParameters);
        }
        public PagedList<TEntity> QueryPage(string sql, int pageIndex, int? pageSize, string orderBy = null, TEntity objParameters = null, CommandType cmdType = CommandType.Text)
        {
            return this.QueryPageImpl<TEntity>(Mapper.EntityType, sql, pageIndex, pageSize, orderBy, cmdType, objParameters);
        }
        public PagedList<TTarget> QueryPage<TTarget>(string sql, int pageIndex, int? pageSize, string orderBy = null, TEntity objParameters = null, CommandType cmdType = CommandType.Text)
        {
            return this.QueryPageImpl<TTarget>(typeof(TTarget), sql, pageIndex, pageSize, orderBy, cmdType, objParameters);
        }
        public int ExecSql(string sql, TEntity objParameters = null, CommandType cmdType = CommandType.Text)
        {
            return this.ExecSqlImpl(sql, cmdType, objParameters);
        }


        public int Update(Action<SqlBuilder> builder, TEntity entity = null)
        {
            return this.ExecSqlImpl(builder, CommandType.Text, entity);
        }
        public TEntity QueryFirst(Action<SqlBuilder> builder, TEntity objParameters = null, CommandType cmdType = CommandType.Text)
        {
            return this.QueryFirstImpl<TEntity>(Mapper.EntityType, builder, cmdType, objParameters);
        }
        public TTarget QueryFirst<TTarget>(Action<SqlBuilder> builder, TEntity objParameters = null, CommandType cmdType = CommandType.Text)
        {
            return this.QueryFirstImpl<TTarget>(typeof(TTarget), builder, cmdType, objParameters);
        }
        public List<TEntity> Query(Action<SqlBuilder> builder, TEntity objParameters = null, CommandType cmdType = CommandType.Text)
        {
            return this.QueryImpl<TEntity>(Mapper.EntityType, builder, cmdType, objParameters);
        }
        public List<TTarget> Query<TTarget>(Action<SqlBuilder> builder, TEntity objParameters = null, CommandType cmdType = CommandType.Text)
        {
            return this.QueryImpl<TTarget>(typeof(TTarget), builder, cmdType, objParameters);
        }
        public int ExecSql(Action<SqlBuilder> builder, TEntity objParameters = null, CommandType cmdType = CommandType.Text)
        {
            return this.ExecSqlImpl(builder, cmdType, objParameters);
        }

        public async Task<TEntity> GetAsync(TEntity key)
        {
            var sql = GetSqlCache(this.ConnString, "GET", this.Provider);
            return await this.QueryFirstImplAsync<TEntity>(Mapper.EntityType, sql, CommandType.Text, key);
        }
        public async Task<int> CreateAsync(TEntity entity)
        {
            var sql = GetSqlCache(this.ConnString, "CREATE", this.Provider);
            return await this.ExecSqlImplAsync(sql, CommandType.Text, entity);
        }
        public async Task<int> DeleteAsync(TEntity key)
        {
            var sql = GetSqlCache(this.ConnString, "DELETE", this.Provider);
            return await this.ExecSqlImplAsync(sql, CommandType.Text, key);
        }
        public async Task<int> UpdateAsync(string sql, TEntity entity = null)
        {
            return await this.ExecSqlImplAsync(sql, CommandType.Text, entity);
        }
        public async Task<int> UpdateAsync<TFields>(Expression<Func<TEntity, TFields>> fieldsExpression, TEntity objParameters = null)
        {
            var sql = GetUpdateFieldsSql(fieldsExpression, this.Provider);
            return await this.ExecSqlImplAsync(sql, CommandType.Text, objParameters);
        }
        public async Task<TEntity> QueryFirstAsync(string sql, TEntity objParameters = null, CommandType cmdType = CommandType.Text)
        {
            return await this.QueryFirstImplAsync<TEntity>(Mapper.EntityType, sql, cmdType, objParameters);
        }
        public async Task<TTarget> QueryFirstAsync<TTarget>(string sql, TEntity objParameters = null, CommandType cmdType = CommandType.Text)
        {
            return await this.QueryFirstImplAsync<TTarget>(typeof(TTarget), sql, cmdType, objParameters);
        }
        public async Task<List<TEntity>> QueryAsync(string sql, TEntity objParameters = null, CommandType cmdType = CommandType.Text)
        {
            return await this.QueryImplAsync<TEntity>(Mapper.EntityType, sql, cmdType, objParameters);
        }
        public async Task<List<TTarget>> QueryAsync<TTarget>(string sql, TEntity objParameters = null, CommandType cmdType = CommandType.Text)
        {
            return await this.QueryImplAsync<TTarget>(typeof(TTarget), sql, cmdType, objParameters);
        }
        public async Task<PagedList<TEntity>> QueryPageAsync(string sql, int pageIndex, int? pageSize, string orderBy = null, TEntity objParameters = null, CommandType cmdType = CommandType.Text)
        {
            return await this.QueryPageImplAsync<TEntity>(Mapper.EntityType, sql, pageIndex, pageSize, orderBy, cmdType, objParameters);
        }
        public async Task<PagedList<TTarget>> QueryPageAsync<TTarget>(string sql, int pageIndex, int? pageSize, string orderBy = null, TEntity objParameters = null, CommandType cmdType = CommandType.Text)
        {
            return await this.QueryPageImplAsync<TTarget>(Mapper.EntityType, sql, pageIndex, pageSize, orderBy, cmdType, objParameters);
        }
        public async Task<int> ExecSqlAsync(string sql, TEntity objParameters = null, CommandType cmdType = CommandType.Text)
        {
            return await this.ExecSqlImplAsync(sql, cmdType, objParameters);
        }

        public async Task<int> UpdateAsync(Action<SqlBuilder> builder, TEntity entity = null)
        {
            return await this.ExecSqlImplAsync(builder, CommandType.Text, entity);
        }
        public async Task<TEntity> QueryFirstAsync(Action<SqlBuilder> builder, TEntity objParameters = null, CommandType cmdType = CommandType.Text)
        {
            return await this.QueryFirstImplAsync<TEntity>(Mapper.EntityType, builder, cmdType, objParameters);
        }
        public async Task<TTarget> QueryFirstAsync<TTarget>(Action<SqlBuilder> builder, TEntity objParameters = null, CommandType cmdType = CommandType.Text)
        {
            return await this.QueryFirstImplAsync<TTarget>(typeof(TTarget), builder, cmdType, objParameters);
        }
        public async Task<List<TEntity>> QueryAsync(Action<SqlBuilder> builder, TEntity objParameters = null, CommandType cmdType = CommandType.Text)
        {
            return await this.QueryImplAsync<TEntity>(Mapper.EntityType, builder, cmdType, objParameters);
        }
        public async Task<List<TTarget>> QueryAsync<TTarget>(Action<SqlBuilder> builder, TEntity objParameters = null, CommandType cmdType = CommandType.Text)
        {
            return await this.QueryImplAsync<TTarget>(typeof(TTarget), builder, cmdType, objParameters);
        }
        public async Task<int> ExecSqlAsync(Action<SqlBuilder> builder, TEntity objParameters = null, CommandType cmdType = CommandType.Text)
        {
            return await this.ExecSqlImplAsync(builder, cmdType, objParameters);
        }
        public void Dispose()
        {
            if (this.Transaction != null) this.Transaction.Dispose();
            if (this.connection != null) this.connection.Dispose();
        }
    }
}
