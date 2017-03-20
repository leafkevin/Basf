using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Basf.Repository
{
    public interface IRepository<TEntity> : IDisposable where TEntity : class
    {
        TEntity Get(TEntity key);
        int Create(TEntity entity);
        int Delete(TEntity key);
        int Update(string sql, TEntity entity = null);
        int Update<TFields>(Expression<Func<TEntity, TFields>> fields, TEntity objParameters = null);
        TEntity QueryFirst(string sql, TEntity objParameters = null, CommandType cmdType = CommandType.Text);
        TTarget QueryFirst<TTarget>(string sql, TEntity objParameters = null, CommandType cmdType = CommandType.Text);
        List<TEntity> Query(string sql, TEntity objParameters = null, CommandType cmdType = CommandType.Text);
        List<TTarget> Query<TTarget>(string sql, TEntity objParameters = null, CommandType cmdType = CommandType.Text);
        PagedList<TEntity> QueryPage(string sql, int pageIndex, int? pageSize, string orderBy = null, TEntity objParameters = null, CommandType cmdType = CommandType.Text);
        PagedList<TTarget> QueryPage<TTarget>(string sql, int pageIndex, int? pageSize, string orderBy = null, TEntity objParameters = null, CommandType cmdType = CommandType.Text);
        int ExecSql(string sql, TEntity objParameters = null, CommandType cmdType = CommandType.Text);

        int Update(Action<SqlBuilder> builder, TEntity entity = null);
        TEntity QueryFirst(Action<SqlBuilder> builder, TEntity objParameters = null, CommandType cmdType = CommandType.Text);
        TTarget QueryFirst<TTarget>(Action<SqlBuilder> builder, TEntity objParameters = null, CommandType cmdType = CommandType.Text);
        List<TEntity> Query(Action<SqlBuilder> builder, TEntity objParameters = null, CommandType cmdType = CommandType.Text);
        List<TTarget> Query<TTarget>(Action<SqlBuilder> builder, TEntity objParameters = null, CommandType cmdType = CommandType.Text);
        int ExecSql(Action<SqlBuilder> builder, TEntity objParameters = null, CommandType cmdType = CommandType.Text);

        Task<TEntity> GetAsync(TEntity key);
        Task<int> CreateAsync(TEntity entity);
        Task<int> DeleteAsync(TEntity key);
        Task<int> UpdateAsync(string sql, TEntity entity = null);
        Task<int> UpdateAsync<TFields>(Expression<Func<TEntity, TFields>> fields, TEntity objParameters = null);
        Task<TEntity> QueryFirstAsync(string sql, TEntity objParameters = null, CommandType cmdType = CommandType.Text);
        Task<TTarget> QueryFirstAsync<TTarget>(string sql, TEntity objParameters = null, CommandType cmdType = CommandType.Text);
        Task<List<TEntity>> QueryAsync(string sql, TEntity objParameters = null, CommandType cmdType = CommandType.Text);
        Task<List<TTarget>> QueryAsync<TTarget>(string sql, TEntity objParameters = null, CommandType cmdType = CommandType.Text);
        Task<PagedList<TEntity>> QueryPageAsync(string sql, int pageIndex, int? pageSize, string orderBy = null, TEntity objParameters = null, CommandType cmdType = CommandType.Text);
        Task<PagedList<TTarget>> QueryPageAsync<TTarget>(string sql, int pageIndex, int? pageSize, string orderBy = null, TEntity objParameters = null, CommandType cmdType = CommandType.Text);
        Task<int> ExecSqlAsync(string sql, TEntity objParameters = null, CommandType cmdType = CommandType.Text);

        Task<int> UpdateAsync(Action<SqlBuilder> builder, TEntity entity = null);
        Task<TEntity> QueryFirstAsync(Action<SqlBuilder> builder, TEntity objParameters = null, CommandType cmdType = CommandType.Text);
        Task<TTarget> QueryFirstAsync<TTarget>(Action<SqlBuilder> builder, TEntity objParameters = null, CommandType cmdType = CommandType.Text);
        Task<List<TEntity>> QueryAsync(Action<SqlBuilder> builder, TEntity objParameters = null, CommandType cmdType = CommandType.Text);
        Task<List<TTarget>> QueryAsync<TTarget>(Action<SqlBuilder> builder, TEntity objParameters = null, CommandType cmdType = CommandType.Text);
        Task<int> ExecSqlAsync(Action<SqlBuilder> builder, TEntity objParameters = null, CommandType cmdType = CommandType.Text);
    }
}
