using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Basf.Repository
{

    public interface IRepository : IDisposable
    {
        TEntity QueryFirst<TEntity>(string sql, object objParameters = null, CommandType cmdType = CommandType.Text);
        List<TEntity> Query<TEntity>(string sql, object objParameters = null, CommandType cmdType = CommandType.Text);
        PagedList<TEntity> QueryPage<TEntity>(string sql, int pageIndex, int? pageSize, string orderBy = null, object objParameters = null, CommandType cmdType = CommandType.Text);
        int ExecSql(string sql, object objParameters = null, CommandType cmdType = CommandType.Text);

        TEntity QueryFirst<TEntity>(Action<SqlBuilder> builder, object objParameters = null, CommandType cmdType = CommandType.Text);
        List<TEntity> Query<TEntity>(Action<SqlBuilder> builder, object objParameters = null, CommandType cmdType = CommandType.Text);
        int ExecSql(Action<SqlBuilder> builder, object objParameters = null, CommandType cmdType = CommandType.Text);

        Task<TEntity> QueryFirstAsync<TEntity>(string sql, object objParameters = null, CommandType cmdType = CommandType.Text);
        Task<List<TEntity>> QueryAsync<TEntity>(string sql, object objParameters = null, CommandType cmdType = CommandType.Text);
        Task<PagedList<TEntity>> QueryPageAsync<TEntity>(string sql, int pageIndex, int? pageSize, string orderBy = null, object objParameters = null, CommandType cmdType = CommandType.Text);
        Task<int> ExecSqlAsync(string sql, object objParameters = null, CommandType cmdType = CommandType.Text);

        Task<TEntity> QueryFirstAsync<TEntity>(Action<SqlBuilder> builder, object objParameters = null, CommandType cmdType = CommandType.Text);
        Task<List<TEntity>> QueryAsync<TEntity>(Action<SqlBuilder> builder, object objParameters = null, CommandType cmdType = CommandType.Text);
        Task<int> ExecSqlAsync(Action<SqlBuilder> builder, object objParameters = null, CommandType cmdType = CommandType.Text);
    }
}
