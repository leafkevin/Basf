using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Basf.Repository
{
    public partial class Repository<TEntity> : IRepository<TEntity> where TEntity : class, new()
    {
        public async Task<TEntity> GetAsync(TEntity key)
        {
            var sql = GetSqlCache(this.ConnString, "GET", this.Provider);
            var action = GetActionCache(this.ConnString, sql, this.Provider, true);
            return await this.QueryFirstImplAsync<TEntity>(Mapper.EntityType, sql, CommandType.Text, key, action);
        }
        public async Task<int> CreateAsync(TEntity entity)
        {
            var sql = GetSqlCache(this.ConnString, "CREATE", this.Provider);
            var action = GetActionCache(this.ConnString, sql, this.Provider, false);
            return await this.ExecSqlImplAsync(sql, entity, action);
        }
        public async Task<int> DeleteAsync(TEntity key)
        {
            var sql = GetSqlCache(this.ConnString, "DELETE", this.Provider);
            var paramAction = GetActionCache(this.ConnString, sql, this.Provider, true);
            return await this.ExecSqlImplAsync(sql, CommandType.Text, key, paramAction);
        }
        public async Task<int> UpdateAsync(string sql, TEntity entity = null)
        {
            var paramAction = GetActionCache(this.ConnString, sql, this.Provider, true);
            return await this.ExecSqlImplAsync(sql, CommandType.Text, entity, paramAction);
        }
        public async Task<int> UpdateAsync<TFields>(Expression<Func<TEntity, TFields>> fieldsExpression, TEntity objParameters = null)
        {
            var sql = GetUpdateFieldsSql(fieldsExpression, this.Provider);
            if (objParameters == null) return this.ExecSqlImpl(sql);
            else
            {
                var paramAction = GetActionCache(this.ConnString, sql, this.Provider, false);
                return await this.ExecSqlImplAsync(sql, CommandType.Text, objParameters, paramAction);
            }
        }
        public async Task<TEntity> QueryFirstAsync(string sql, TEntity objParameters = null, CommandType cmdType = CommandType.Text)
        {
            if (objParameters == null) return await this.QueryFirstImplAsync<TEntity>(Mapper.EntityType, sql, cmdType);
            else
            {
                var paramAction = GetActionCache(this.ConnString, sql, this.Provider, false);
                return await this.QueryFirstImplAsync<TEntity>(Mapper.EntityType, sql, cmdType, objParameters, paramAction);
            }
        }
        public async Task<TTarget> QueryFirstAsync<TTarget>(string sql, TEntity objParameters = null, CommandType cmdType = CommandType.Text)
        {
            if (objParameters == null) return await this.QueryFirstImplAsync<TTarget>(typeof(TTarget), sql, cmdType);
            else
            {
                var paramAction = GetActionCache(this.ConnString, sql, this.Provider, false);
                return await this.QueryFirstImplAsync<TTarget>(typeof(TTarget), sql, cmdType, objParameters, paramAction);
            }
        }
        public async Task<List<TEntity>> QueryAsync(string sql, TEntity objParameters = null, CommandType cmdType = CommandType.Text)
        {
            if (objParameters == null) return await this.QueryImplAsync<TEntity>(Mapper.EntityType, sql, cmdType);
            else
            {
                var paramAction = GetActionCache(this.ConnString, sql, this.Provider, false);
                return await this.QueryImplAsync<TEntity>(Mapper.EntityType, sql, cmdType, objParameters, paramAction);
            }
        }
        public async Task<List<TTarget>> QueryAsync<TTarget>(string sql, TEntity objParameters = null, CommandType cmdType = CommandType.Text)
        {
            if (objParameters == null) return await this.QueryImplAsync<TTarget>(typeof(TTarget), sql, cmdType);
            else
            {
                var paramAction = GetActionCache(this.ConnString, sql, this.Provider, false);
                return await this.QueryImplAsync<TTarget>(typeof(TTarget), sql, cmdType, objParameters, paramAction);
            }
        }
        public async Task<PagedList<TEntity>> QueryPageAsync(string sql, int pageIndex, int? pageSize, string orderBy = null, TEntity objParameters = null, CommandType cmdType = CommandType.Text)
        {
            sql = RepositoryHelper.GetPagingCache(this.ConnString, sql, pageIndex, pageSize, orderBy, this.Provider);
            if (objParameters == null) return await this.QueryPageImplAsync<TEntity>(Mapper.EntityType, sql, cmdType);
            else
            {
                var paramAction = GetActionCache(this.ConnString, sql, this.Provider, false);
                return await this.QueryPageImplAsync<TEntity>(Mapper.EntityType, sql, cmdType, objParameters, paramAction);
            }
        }
        public async Task<PagedList<TTarget>> QueryPageAsync<TTarget>(string sql, int pageIndex, int? pageSize, string orderBy = null, TEntity objParameters = null, CommandType cmdType = CommandType.Text)
        {
            sql = RepositoryHelper.GetPagingCache(this.ConnString, sql, pageIndex, pageSize, orderBy, this.Provider);
            if (objParameters == null) return await this.QueryPageImplAsync<TTarget>(Mapper.EntityType, sql, cmdType);
            else
            {
                var paramAction = GetActionCache(this.ConnString, sql, this.Provider, false);
                return await this.QueryPageImplAsync<TTarget>(Mapper.EntityType, sql, cmdType, objParameters, paramAction);
            }
        }
        public async Task<int> ExecSqlAsync(string sql, TEntity objParameters = null, CommandType cmdType = CommandType.Text)
        {
            if (objParameters == null) return await this.ExecSqlImplAsync(sql, cmdType);
            else
            {
                var paramAction = GetActionCache(this.ConnString, sql, this.Provider, false);
                return await this.ExecSqlImplAsync(sql, cmdType, objParameters, paramAction);
            }
        }
        private async Task<TTarget> QueryFirstImplAsync<TTarget>(Type targetType, string sql, CommandType cmdType, TEntity objParameters = null, Action<IDbCommand, TEntity> paramAction = null)
        {
            DbCommand command = null;
            string propName = String.Empty;
            DbDataReader reader = null;
            TTarget result = default(TTarget);
            if (this.connection == null)
            {
                using (var conn = this.Provider.CreateConnection(this.ConnString))
                {
                    command = conn.CreateCommand();
                    command.CommandText = sql;
                    command.CommandType = cmdType;
                    if (objParameters != null) paramAction(command, objParameters);
                    conn.Open();
                    reader = await command.ExecuteReaderAsync(CommandBehavior.CloseConnection | CommandBehavior.SequentialAccess | CommandBehavior.SingleResult | CommandBehavior.SingleRow);
                    var func = RepositoryHelper.GetReader(targetType, reader);
                    while (reader.Read())
                    {
                        object funcResult = func?.Invoke(reader);
                        if (funcResult == null || funcResult is TTarget) result = (TTarget)funcResult;
                        else result = (TTarget)Convert.ChangeType(funcResult, targetType, CultureInfo.InvariantCulture);
                    }
                    while (reader.NextResult()) { }
                    conn.Close();
                    reader.Dispose();
                    reader = null;
                }
            }
            else
            {
                command = this.connection.CreateCommand();
                command.CommandText = sql;
                command.CommandType = CommandType.Text;
                if (objParameters != null) paramAction(command, objParameters);
                if (this.Transaction != null) command.Transaction = this.Transaction;
                reader = await command.ExecuteReaderAsync(CommandBehavior.SequentialAccess | CommandBehavior.SingleResult | CommandBehavior.SingleRow);
                var func = RepositoryHelper.GetReader(targetType, reader);
                while (reader.Read())
                {
                    object funcResult = func?.Invoke(reader);
                    if (funcResult == null || funcResult is TTarget) result = (TTarget)funcResult;
                    else result = (TTarget)Convert.ChangeType(funcResult, targetType, CultureInfo.InvariantCulture);
                }
                while (reader.NextResult()) { }
                reader.Dispose();
                reader = null;
            }
            return result;
        }
        private async Task<List<TTarget>> QueryImplAsync<TTarget>(Type targetType, string sql, CommandType cmdType, TEntity objParameters = null, Action<IDbCommand, TEntity> paramAction = null)
        {
            DbCommand command = null;
            string propName = String.Empty;
            DbDataReader reader = null;
            List<TTarget> result = new List<TTarget>();
            if (this.connection == null)
            {
                using (var conn = this.Provider.CreateConnection(this.ConnString))
                {
                    command = conn.CreateCommand();
                    command.CommandText = sql;
                    command.CommandType = cmdType;
                    if (objParameters != null) paramAction(command, objParameters);
                    conn.Open();
                    reader = await command.ExecuteReaderAsync(CommandBehavior.CloseConnection | CommandBehavior.SequentialAccess);
                    var func = RepositoryHelper.GetReader(targetType, reader);
                    while (reader.Read())
                    {
                        object funcResult = func?.Invoke(reader);
                        if (funcResult == null) continue;
                        if (funcResult is TTarget) result.Add((TTarget)funcResult);
                        else result.Add((TTarget)Convert.ChangeType(funcResult, targetType, CultureInfo.InvariantCulture));
                    }
                    while (reader.NextResult()) { }
                    conn.Close();
                    reader.Dispose();
                    reader = null;
                }
            }
            else
            {
                command = this.connection.CreateCommand();
                command.CommandText = sql;
                command.CommandType = CommandType.Text;
                if (objParameters != null) paramAction(command, objParameters);
                if (this.Transaction != null) command.Transaction = this.Transaction;
                reader = await command.ExecuteReaderAsync(CommandBehavior.SequentialAccess);
                var func = RepositoryHelper.GetReader(targetType, reader);
                while (reader.Read())
                {
                    object funcResult = func?.Invoke(reader);
                    if (funcResult == null) continue;
                    if (funcResult is TTarget) result.Add((TTarget)funcResult);
                    else result.Add((TTarget)Convert.ChangeType(funcResult, targetType, CultureInfo.InvariantCulture));
                }
                while (reader.NextResult()) { }
                reader.Dispose();
                reader = null;
            }
            return result;
        }
        private async Task<PagedList<TTarget>> QueryPageImplAsync<TTarget>(Type targetType, string sql, CommandType cmdType, TEntity objParameters = null, Action<IDbCommand, TEntity> paramAction = null)
        {
            DbCommand command = null;
            string propName = String.Empty;
            DbDataReader reader = null;
            PagedList<TTarget> result = new PagedList<TTarget>();
            if (this.connection == null)
            {
                using (var conn = this.Provider.CreateConnection(this.ConnString))
                {
                    command = conn.CreateCommand();
                    command.CommandText = sql;
                    command.CommandType = cmdType;
                    if (objParameters != null) paramAction(command, objParameters);
                    conn.Open();
                    reader = await command.ExecuteReaderAsync(CommandBehavior.CloseConnection | CommandBehavior.SequentialAccess);
                    var func = RepositoryHelper.GetReader(targetType, reader);
                    while (reader.Read())
                    {
                        object funcResult = func?.Invoke(reader);
                        if (funcResult == null) continue;
                        if (funcResult is TTarget) result.Add((TTarget)funcResult);
                        else result.Add((TTarget)Convert.ChangeType(funcResult, targetType, CultureInfo.InvariantCulture));
                    }
                    while (reader.NextResult()) { }
                    conn.Close();
                    reader.Dispose();
                    reader = null;
                }
            }
            else
            {
                command = this.connection.CreateCommand();
                command.CommandText = sql;
                command.CommandType = CommandType.Text;
                if (objParameters != null) paramAction(command, objParameters);
                if (this.Transaction != null) command.Transaction = this.Transaction;
                reader = await command.ExecuteReaderAsync(CommandBehavior.SequentialAccess);
                var func = RepositoryHelper.GetReader(targetType, reader);
                while (reader.Read())
                {
                    object funcResult = func?.Invoke(reader);
                    if (funcResult == null) continue;
                    if (funcResult is TTarget) result.Add((TTarget)funcResult);
                    else result.Add((TTarget)Convert.ChangeType(funcResult, targetType, CultureInfo.InvariantCulture));
                }
                while (reader.NextResult()) { }
                reader.Dispose();
                reader = null;
            }
            return result;
        }
        private async Task<int> ExecSqlImplAsync(string sql, CommandType cmdType = CommandType.Text, TEntity objParameters = null, Action<IDbCommand, TEntity> paramAction = null)
        {
            DbCommand command = null;
            int result = 0;
            if (this.connection == null)
            {
                using (var conn = this.Provider.CreateConnection(this.ConnString))
                {
                    command = conn.CreateCommand();
                    command.CommandText = sql;
                    command.CommandType = cmdType;
                    if (objParameters != null) paramAction(command, objParameters);
                    conn.Open();
                    result = await command.ExecuteNonQueryAsync();
                    conn.Close();
                }
            }
            else
            {
                command = this.connection.CreateCommand();
                command.CommandText = sql;
                command.CommandType = cmdType;
                if (this.Transaction != null) command.Transaction = this.Transaction;
                if (objParameters != null) paramAction(command, objParameters);
                this.Open();
                result = await command.ExecuteNonQueryAsync();
            }
            return result;
        }
    }
}
