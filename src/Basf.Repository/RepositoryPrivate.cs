using System;
using System.Collections.Concurrent;
using System.Text;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Globalization;
using System.Data.Common;

namespace Basf.Repository
{
    public partial class Repository<TEntity> : IRepository<TEntity> where TEntity : class, new()
    {
        private static ConcurrentDictionary<int, Action<IDbCommand, TEntity>> ActionCache = new ConcurrentDictionary<int, Action<IDbCommand, TEntity>>();
        private static ConcurrentDictionary<int, string> SqlCache = new ConcurrentDictionary<int, string>();

        private TTarget QueryFirstImpl<TTarget>(Type targetType, string sql, CommandType cmdType, TEntity objParameters = null, bool isPk = false)
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
                    int hashKey = RepositoryHelper.GetHashKey(this.ConnString, sql);
                    if (objParameters != null)
                    {
                        var paramAction = GetActionCache(hashKey, sql, this.Provider, isPk);
                        paramAction(command, objParameters);
                    }
                    conn.Open();
                    reader = command.ExecuteReader(CommandBehavior.CloseConnection | CommandBehavior.SequentialAccess | CommandBehavior.SingleResult | CommandBehavior.SingleRow);
                    var func = RepositoryHelper.GetReader(hashKey, targetType, reader);
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
                if (this.Transaction != null) command.Transaction = this.Transaction;
                int hashKey = RepositoryHelper.GetHashKey(this.ConnString, sql);
                if (objParameters != null)
                {
                    var paramAction = GetActionCache(hashKey, sql, this.Provider, isPk);
                    paramAction(command, objParameters);
                }
                this.Open();
                reader = command.ExecuteReader(CommandBehavior.SequentialAccess | CommandBehavior.SingleResult | CommandBehavior.SingleRow);
                var func = RepositoryHelper.GetReader(hashKey, targetType, reader);
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
        private List<TTarget> QueryImpl<TTarget>(Type targetType, string sql, CommandType cmdType, TEntity objParameters = null, bool isPk = false)
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
                    int hashKey = RepositoryHelper.GetHashKey(this.ConnString, sql);
                    if (objParameters != null)
                    {
                        var paramAction = GetActionCache(hashKey, sql, this.Provider, isPk);
                        paramAction(command, objParameters);
                    }
                    conn.Open();
                    reader = command.ExecuteReader(CommandBehavior.CloseConnection | CommandBehavior.SequentialAccess);
                    var func = RepositoryHelper.GetReader(hashKey, targetType, reader);
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
                if (this.Transaction != null) command.Transaction = this.Transaction;
                int hashKey = RepositoryHelper.GetHashKey(this.ConnString, sql);
                if (objParameters != null)
                {
                    var paramAction = GetActionCache(hashKey, sql, this.Provider, isPk);
                    paramAction(command, objParameters);
                }
                this.Open();
                reader = command.ExecuteReader(CommandBehavior.SequentialAccess);
                var func = RepositoryHelper.GetReader(hashKey, targetType, reader);
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
        private PagedList<TTarget> QueryPageImpl<TTarget>(Type targetType, string sql, int pageIndex, int? pageSize, string orderBy, CommandType cmdType, TEntity objParameters = null, bool isPk = false)
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
                    int hashKey = RepositoryHelper.GetHashKey(this.ConnString, sql + orderBy ?? "");
                    sql = RepositoryHelper.GetPagingCache(hashKey, this.ConnString, sql, pageIndex, pageSize, orderBy, this.Provider);
                    if (objParameters != null)
                    {
                        var paramAction = GetActionCache(hashKey, sql, this.Provider, isPk);
                        paramAction(command, objParameters);
                    }
                    conn.Open();
                    reader = command.ExecuteReader(CommandBehavior.CloseConnection | CommandBehavior.SequentialAccess);
                    var func = RepositoryHelper.GetReader(hashKey, targetType, reader);
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
                if (this.Transaction != null) command.Transaction = this.Transaction;
                int hashKey = RepositoryHelper.GetHashKey(this.ConnString, sql + orderBy ?? "");
                sql = RepositoryHelper.GetPagingCache(hashKey, this.ConnString, sql, pageIndex, pageSize, orderBy, this.Provider);
                if (objParameters != null)
                {
                    var paramAction = GetActionCache(hashKey, sql, this.Provider, isPk);
                    paramAction(command, objParameters);
                }
                this.Open();
                reader = command.ExecuteReader(CommandBehavior.SequentialAccess);
                var func = RepositoryHelper.GetReader(hashKey, targetType, reader);
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
        private int ExecSqlImpl(string sql, CommandType cmdType = CommandType.Text, TEntity objParameters = null, bool isPk = false)
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
                    int hashKey = RepositoryHelper.GetHashKey(this.ConnString, sql);
                    if (objParameters != null)
                    {
                        var paramAction = GetActionCache(hashKey, sql, this.Provider, isPk);
                        paramAction(command, objParameters);
                    }
                    conn.Open();
                    result = command.ExecuteNonQuery();
                    conn.Close();
                }
            }
            else
            {
                command = this.connection.CreateCommand();
                command.CommandText = sql;
                command.CommandType = cmdType;
                if (this.Transaction != null) command.Transaction = this.Transaction;
                int hashKey = RepositoryHelper.GetHashKey(this.ConnString, sql);
                if (objParameters != null)
                {
                    var paramAction = GetActionCache(hashKey, sql, this.Provider, isPk);
                    paramAction(command, objParameters);
                }
                this.Open();
                result = command.ExecuteNonQuery();
            }
            return result;
        }
        private async Task<TTarget> QueryFirstImplAsync<TTarget>(Type targetType, string sql, CommandType cmdType, TEntity objParameters = null, bool isPk = false)
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
                    int hashKey = RepositoryHelper.GetHashKey(this.ConnString, sql);
                    if (objParameters != null)
                    {
                        var paramAction = GetActionCache(hashKey, sql, this.Provider, isPk);
                        paramAction(command, objParameters);
                    }
                    conn.Open();
                    reader = await command.ExecuteReaderAsync(CommandBehavior.CloseConnection | CommandBehavior.SequentialAccess | CommandBehavior.SingleResult | CommandBehavior.SingleRow);
                    var func = RepositoryHelper.GetReader(hashKey, targetType, reader);
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
                if (this.Transaction != null) command.Transaction = this.Transaction;
                int hashKey = RepositoryHelper.GetHashKey(this.ConnString, sql);
                if (objParameters != null)
                {
                    var paramAction = GetActionCache(hashKey, sql, this.Provider, isPk);
                    paramAction(command, objParameters);
                }
                this.Open();
                reader = await command.ExecuteReaderAsync(CommandBehavior.SequentialAccess | CommandBehavior.SingleResult | CommandBehavior.SingleRow);
                var func = RepositoryHelper.GetReader(hashKey, targetType, reader);
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
        private async Task<List<TTarget>> QueryImplAsync<TTarget>(Type targetType, string sql, CommandType cmdType, TEntity objParameters = null, bool isPk = false)
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
                    int hashKey = RepositoryHelper.GetHashKey(this.ConnString, sql);
                    if (objParameters != null)
                    {
                        var paramAction = GetActionCache(hashKey, sql, this.Provider, isPk);
                        paramAction(command, objParameters);
                    }
                    conn.Open();
                    reader = await command.ExecuteReaderAsync(CommandBehavior.CloseConnection | CommandBehavior.SequentialAccess);
                    var func = RepositoryHelper.GetReader(hashKey, targetType, reader);
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
                if (this.Transaction != null) command.Transaction = this.Transaction;
                int hashKey = RepositoryHelper.GetHashKey(this.ConnString, sql);
                if (objParameters != null)
                {
                    var paramAction = GetActionCache(hashKey, sql, this.Provider, isPk);
                    paramAction(command, objParameters);
                }
                this.Open();
                reader = await command.ExecuteReaderAsync(CommandBehavior.SequentialAccess);
                var func = RepositoryHelper.GetReader(hashKey, targetType, reader);
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
        private async Task<PagedList<TTarget>> QueryPageImplAsync<TTarget>(Type targetType, string sql, int pageIndex, int? pageSize, string orderBy, CommandType cmdType, TEntity objParameters = null, bool isPk = false)
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
                    int hashKey = RepositoryHelper.GetHashKey(this.ConnString, sql);
                    sql = RepositoryHelper.GetPagingCache(hashKey, this.ConnString, sql, pageIndex, pageSize, orderBy, this.Provider);
                    if (objParameters != null)
                    {
                        var paramAction = GetActionCache(hashKey, sql, this.Provider, isPk);
                        paramAction(command, objParameters);
                    }
                    conn.Open();
                    reader = await command.ExecuteReaderAsync(CommandBehavior.CloseConnection | CommandBehavior.SequentialAccess);
                    var func = RepositoryHelper.GetReader(hashKey, targetType, reader);
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
                if (this.Transaction != null) command.Transaction = this.Transaction;
                int hashKey = RepositoryHelper.GetHashKey(this.ConnString, sql);
                sql = RepositoryHelper.GetPagingCache(hashKey, this.ConnString, sql, pageIndex, pageSize, orderBy, this.Provider);
                if (objParameters != null)
                {
                    var paramAction = GetActionCache(hashKey, sql, this.Provider, isPk);
                    paramAction(command, objParameters);
                }
                this.Open();
                reader = await command.ExecuteReaderAsync(CommandBehavior.SequentialAccess);
                var func = RepositoryHelper.GetReader(hashKey, targetType, reader);
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
        private async Task<int> ExecSqlImplAsync(string sql, CommandType cmdType = CommandType.Text, TEntity objParameters = null, bool isPk = false)
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
                    int hashKey = RepositoryHelper.GetHashKey(this.ConnString, sql);
                    if (objParameters != null)
                    {
                        var paramAction = GetActionCache(hashKey, sql, this.Provider, isPk);
                        paramAction(command, objParameters);
                    }
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
                int hashKey = RepositoryHelper.GetHashKey(this.ConnString, sql);
                if (objParameters != null)
                {
                    var paramAction = GetActionCache(hashKey, sql, this.Provider, isPk);
                    paramAction(command, objParameters);
                }
                this.Open();
                result = await command.ExecuteNonQueryAsync();
            }
            return result;
        }

        private TTarget QueryFirstImpl<TTarget>(Type targetType, Action<SqlBuilder> builder, CommandType cmdType, TEntity objParameters = null, bool isPk = false)
        {
            DbCommand command = null;
            string propName = String.Empty;
            DbDataReader reader = null;
            TTarget result = default(TTarget);
            SqlBuilder sqlBuilder = new SqlBuilder(this.Provider);
            builder.Invoke(sqlBuilder);
            var sql = sqlBuilder.BuildSql();
            if (this.connection == null)
            {
                using (var conn = this.Provider.CreateConnection(this.ConnString))
                {
                    command = conn.CreateCommand();
                    command.CommandText = sql;
                    command.CommandType = cmdType;
                    int hashKey = RepositoryHelper.GetHashKey(this.ConnString, sql);
                    if (objParameters != null)
                    {
                        var paramAction = GetActionCache(hashKey, sql, this.Provider, isPk);
                        paramAction(command, objParameters);
                    }
                    conn.Open();
                    reader = command.ExecuteReader(CommandBehavior.CloseConnection | CommandBehavior.SequentialAccess | CommandBehavior.SingleResult | CommandBehavior.SingleRow);
                    var func = RepositoryHelper.GetReader(hashKey, targetType, reader);
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
                if (this.Transaction != null) command.Transaction = this.Transaction;
                int hashKey = RepositoryHelper.GetHashKey(this.ConnString, sql);
                if (objParameters != null)
                {
                    var paramAction = GetActionCache(hashKey, sql, this.Provider, isPk);
                    paramAction(command, objParameters);
                }
                this.Open();
                reader = command.ExecuteReader(CommandBehavior.SequentialAccess | CommandBehavior.SingleResult | CommandBehavior.SingleRow);
                var func = RepositoryHelper.GetReader(hashKey, targetType, reader);
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
        private List<TTarget> QueryImpl<TTarget>(Type targetType, Action<SqlBuilder> builder, CommandType cmdType, TEntity objParameters = null, bool isPk = false)
        {
            DbCommand command = null;
            string propName = String.Empty;
            DbDataReader reader = null;
            List<TTarget> result = new List<TTarget>();
            SqlBuilder sqlBuilder = new SqlBuilder(this.Provider);
            builder.Invoke(sqlBuilder);
            var sql = sqlBuilder.BuildSql();
            if (this.connection == null)
            {
                using (var conn = this.Provider.CreateConnection(this.ConnString))
                {
                    command = conn.CreateCommand();
                    command.CommandText = sql;
                    command.CommandType = cmdType;
                    int hashKey = RepositoryHelper.GetHashKey(this.ConnString, sql);
                    if (objParameters != null)
                    {
                        var paramAction = GetActionCache(hashKey, sql, this.Provider, isPk);
                        paramAction(command, objParameters);
                    }
                    conn.Open();
                    reader = command.ExecuteReader(CommandBehavior.CloseConnection | CommandBehavior.SequentialAccess);
                    var func = RepositoryHelper.GetReader(hashKey, targetType, reader);
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
                if (this.Transaction != null) command.Transaction = this.Transaction;
                int hashKey = RepositoryHelper.GetHashKey(this.ConnString, sql);
                if (objParameters != null)
                {
                    var paramAction = GetActionCache(hashKey, sql, this.Provider, isPk);
                    paramAction(command, objParameters);
                }
                this.Open();
                reader = command.ExecuteReader(CommandBehavior.SequentialAccess);
                var func = RepositoryHelper.GetReader(hashKey, targetType, reader);
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
        private PagedList<TTarget> QueryPageImpl<TTarget>(Type targetType, Action<SqlBuilder> builder, int pageIndex, int? pageSize, string orderBy, CommandType cmdType, TEntity objParameters = null, bool isPk = false)
        {
            DbCommand command = null;
            string propName = String.Empty;
            DbDataReader reader = null;
            PagedList<TTarget> result = new PagedList<TTarget>();
            SqlBuilder sqlBuilder = new SqlBuilder(this.Provider);
            builder.Invoke(sqlBuilder);
            var sql = sqlBuilder.BuildSql();
            if (this.connection == null)
            {
                using (var conn = this.Provider.CreateConnection(this.ConnString))
                {
                    command = conn.CreateCommand();
                    command.CommandText = sql;
                    command.CommandType = cmdType;
                    int hashKey = RepositoryHelper.GetHashKey(this.ConnString, sql + orderBy ?? "");
                    sql = RepositoryHelper.GetPagingCache(hashKey, this.ConnString, sql, pageIndex, pageSize, orderBy, this.Provider);
                    if (objParameters != null)
                    {
                        var paramAction = GetActionCache(hashKey, sql, this.Provider, isPk);
                        paramAction(command, objParameters);
                    }
                    conn.Open();
                    reader = command.ExecuteReader(CommandBehavior.CloseConnection | CommandBehavior.SequentialAccess);
                    var func = RepositoryHelper.GetReader(hashKey, targetType, reader);
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
                if (this.Transaction != null) command.Transaction = this.Transaction;
                int hashKey = RepositoryHelper.GetHashKey(this.ConnString, sql + orderBy ?? "");
                sql = RepositoryHelper.GetPagingCache(hashKey, this.ConnString, sql, pageIndex, pageSize, orderBy, this.Provider);
                if (objParameters != null)
                {
                    var paramAction = GetActionCache(hashKey, sql, this.Provider, isPk);
                    paramAction(command, objParameters);
                }
                this.Open();
                reader = command.ExecuteReader(CommandBehavior.SequentialAccess);
                var func = RepositoryHelper.GetReader(hashKey, targetType, reader);
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
        private int ExecSqlImpl(Action<SqlBuilder> builder, CommandType cmdType = CommandType.Text, TEntity objParameters = null, bool isPk = false)
        {
            DbCommand command = null;
            int result = 0;
            SqlBuilder sqlBuilder = new SqlBuilder(this.Provider);
            builder.Invoke(sqlBuilder);
            var sql = sqlBuilder.BuildSql();
            if (this.connection == null)
            {
                using (var conn = this.Provider.CreateConnection(this.ConnString))
                {
                    command = conn.CreateCommand();
                    command.CommandText = sql;
                    command.CommandType = cmdType;
                    int hashKey = RepositoryHelper.GetHashKey(this.ConnString, sql);
                    if (objParameters != null)
                    {
                        var paramAction = GetActionCache(hashKey, sql, this.Provider, isPk);
                        paramAction(command, objParameters);
                    }
                    conn.Open();
                    result = command.ExecuteNonQuery();
                    conn.Close();
                }
            }
            else
            {
                command = this.connection.CreateCommand();
                command.CommandText = sql;
                command.CommandType = cmdType;
                if (this.Transaction != null) command.Transaction = this.Transaction;
                int hashKey = RepositoryHelper.GetHashKey(this.ConnString, sql);
                if (objParameters != null)
                {
                    var paramAction = GetActionCache(hashKey, sql, this.Provider, isPk);
                    paramAction(command, objParameters);
                }
                this.Open();
                result = command.ExecuteNonQuery();
            }
            return result;
        }
        private async Task<TTarget> QueryFirstImplAsync<TTarget>(Type targetType, Action<SqlBuilder> builder, CommandType cmdType, TEntity objParameters = null, bool isPk = false)
        {
            DbCommand command = null;
            string propName = String.Empty;
            DbDataReader reader = null;
            TTarget result = default(TTarget);
            SqlBuilder sqlBuilder = new SqlBuilder(this.Provider);
            builder.Invoke(sqlBuilder);
            var sql = sqlBuilder.BuildSql();
            if (this.connection == null)
            {
                using (var conn = this.Provider.CreateConnection(this.ConnString))
                {
                    command = conn.CreateCommand();
                    command.CommandText = sql;
                    command.CommandType = cmdType;
                    int hashKey = RepositoryHelper.GetHashKey(this.ConnString, sql);
                    if (objParameters != null)
                    {
                        var paramAction = GetActionCache(hashKey, sql, this.Provider, isPk);
                        paramAction(command, objParameters);
                    }
                    conn.Open();
                    reader = await command.ExecuteReaderAsync(CommandBehavior.CloseConnection | CommandBehavior.SequentialAccess | CommandBehavior.SingleResult | CommandBehavior.SingleRow);
                    var func = RepositoryHelper.GetReader(hashKey, targetType, reader);
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
                if (this.Transaction != null) command.Transaction = this.Transaction;
                int hashKey = RepositoryHelper.GetHashKey(this.ConnString, sql);
                if (objParameters != null)
                {
                    var paramAction = GetActionCache(hashKey, sql, this.Provider, isPk);
                    paramAction(command, objParameters);
                }
                this.Open();
                reader = await command.ExecuteReaderAsync(CommandBehavior.SequentialAccess | CommandBehavior.SingleResult | CommandBehavior.SingleRow);
                var func = RepositoryHelper.GetReader(hashKey, targetType, reader);
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
        private async Task<List<TTarget>> QueryImplAsync<TTarget>(Type targetType, Action<SqlBuilder> builder, CommandType cmdType, TEntity objParameters = null, bool isPk = false)
        {
            DbCommand command = null;
            string propName = String.Empty;
            DbDataReader reader = null;
            List<TTarget> result = new List<TTarget>();
            SqlBuilder sqlBuilder = new SqlBuilder(this.Provider);
            builder.Invoke(sqlBuilder);
            var sql = sqlBuilder.BuildSql();
            if (this.connection == null)
            {
                using (var conn = this.Provider.CreateConnection(this.ConnString))
                {
                    command = conn.CreateCommand();
                    command.CommandText = sql;
                    command.CommandType = cmdType;
                    int hashKey = RepositoryHelper.GetHashKey(this.ConnString, sql);
                    if (objParameters != null)
                    {
                        var paramAction = GetActionCache(hashKey, sql, this.Provider, isPk);
                        paramAction(command, objParameters);
                    }
                    conn.Open();
                    reader = await command.ExecuteReaderAsync(CommandBehavior.CloseConnection | CommandBehavior.SequentialAccess);
                    var func = RepositoryHelper.GetReader(hashKey, targetType, reader);
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
                if (this.Transaction != null) command.Transaction = this.Transaction;
                int hashKey = RepositoryHelper.GetHashKey(this.ConnString, sql);
                if (objParameters != null)
                {
                    var paramAction = GetActionCache(hashKey, sql, this.Provider, isPk);
                    paramAction(command, objParameters);
                }
                this.Open();
                reader = await command.ExecuteReaderAsync(CommandBehavior.SequentialAccess);
                var func = RepositoryHelper.GetReader(hashKey, targetType, reader);
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
        private async Task<PagedList<TTarget>> QueryPageImplAsync<TTarget>(Type targetType, Action<SqlBuilder> builder, int pageIndex, int? pageSize, string orderBy, CommandType cmdType, TEntity objParameters = null, bool isPk = false)
        {
            DbCommand command = null;
            string propName = String.Empty;
            DbDataReader reader = null;
            PagedList<TTarget> result = new PagedList<TTarget>();
            SqlBuilder sqlBuilder = new SqlBuilder(this.Provider);
            builder.Invoke(sqlBuilder);
            var sql = sqlBuilder.BuildSql();
            if (this.connection == null)
            {
                using (var conn = this.Provider.CreateConnection(this.ConnString))
                {
                    command = conn.CreateCommand();
                    command.CommandText = sql;
                    command.CommandType = cmdType;
                    int hashKey = RepositoryHelper.GetHashKey(this.ConnString, sql);
                    sql = RepositoryHelper.GetPagingCache(hashKey, this.ConnString, sql, pageIndex, pageSize, orderBy, this.Provider);
                    if (objParameters != null)
                    {
                        var paramAction = GetActionCache(hashKey, sql, this.Provider, isPk);
                        paramAction(command, objParameters);
                    }
                    conn.Open();
                    reader = await command.ExecuteReaderAsync(CommandBehavior.CloseConnection | CommandBehavior.SequentialAccess);
                    var func = RepositoryHelper.GetReader(hashKey, targetType, reader);
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
                if (this.Transaction != null) command.Transaction = this.Transaction;
                int hashKey = RepositoryHelper.GetHashKey(this.ConnString, sql);
                sql = RepositoryHelper.GetPagingCache(hashKey, this.ConnString, sql, pageIndex, pageSize, orderBy, this.Provider);
                if (objParameters != null)
                {
                    var paramAction = GetActionCache(hashKey, sql, this.Provider, isPk);
                    paramAction(command, objParameters);
                }
                this.Open();
                reader = await command.ExecuteReaderAsync(CommandBehavior.SequentialAccess);
                var func = RepositoryHelper.GetReader(hashKey, targetType, reader);
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
        private async Task<int> ExecSqlImplAsync(Action<SqlBuilder> builder, CommandType cmdType = CommandType.Text, TEntity objParameters = null, bool isPk = false)
        {
            DbCommand command = null;
            int result = 0;
            SqlBuilder sqlBuilder = new SqlBuilder(this.Provider);
            builder.Invoke(sqlBuilder);
            var sql = sqlBuilder.BuildSql();
            if (this.connection == null)
            {
                using (var conn = this.Provider.CreateConnection(this.ConnString))
                {
                    command = conn.CreateCommand();
                    command.CommandText = sql;
                    command.CommandType = cmdType;
                    int hashKey = RepositoryHelper.GetHashKey(this.ConnString, sql);
                    if (objParameters != null)
                    {
                        var paramAction = GetActionCache(hashKey, sql, this.Provider, isPk);
                        paramAction(command, objParameters);
                    }
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
                int hashKey = RepositoryHelper.GetHashKey(this.ConnString, sql);
                if (objParameters != null)
                {
                    var paramAction = GetActionCache(hashKey, sql, this.Provider, isPk);
                    paramAction(command, objParameters);
                }
                this.Open();
                result = await command.ExecuteNonQueryAsync();
            }
            return result;
        }

        private static string GetUpdateFieldsSql<TFields>(Expression<Func<TEntity, TFields>> fieldsExpression, IOrmProvider provider)
        {
            var builder = new StringBuilder("UPDATE " + Mapper.TableName + " SET ");
            var expression = ((LambdaExpression)fieldsExpression).Body;
            if (expression.NodeType == ExpressionType.New)
            {
                var newExpression = expression as NewExpression;
                int index = 0;
                foreach (var item in newExpression.Arguments)
                {
                    if (item.NodeType == ExpressionType.MemberAccess)
                    {
                        var exp = item as MemberExpression;
                        if (index > 0) builder.Append(",");
                        builder.Append(GetSetParameterSql(provider, Mapper.MemberMappers[exp.Member.Name]));
                        index++;
                    }
                    else throw new Exception("不支持的Linq表达式");
                }
            }
            else if (expression.NodeType == ExpressionType.MemberAccess)
            {
                var exp = expression as MemberExpression;
                builder.Append(GetSetParameterSql(provider, Mapper.MemberMappers[exp.Member.Name]));
            }
            else throw new Exception("不支持的Linq表达式");
            builder.Append(" WHERE ");
            foreach (var colMapper in Mapper.PrimaryKeys)
            {
                builder.Append(GetSetParameterSql(provider, colMapper));
            }
            return builder.ToString();
        }
        private void Open()
        {
            if (this.connection.State == ConnectionState.Broken) this.connection.Close();
            if (this.connection.State == ConnectionState.Closed) this.connection.Open();
        }
        private static string GetSqlCache(string connString, string sqlKey, IOrmProvider provider)
        {
            string result = sqlKey;
            int cacheKey = RepositoryHelper.GetHashKey(connString, sqlKey);
            switch (sqlKey)
            {
                case "GET":
                    if (!SqlCache.TryGetValue(cacheKey, out result))
                    {
                        result = BuildGetSql(Mapper, provider);
                        SqlCache.TryAdd(cacheKey, result);
                    }
                    break;
                case "CREATE":
                    if (!SqlCache.TryGetValue(cacheKey, out result))
                    {
                        result = BuildCreateSql(Mapper, provider);
                        SqlCache.TryAdd(cacheKey, result);
                    }
                    break;
                case "DELETE":
                    if (!SqlCache.TryGetValue(cacheKey, out result))
                    {
                        result = BuildDeleteSql(Mapper, provider);
                        SqlCache.TryAdd(cacheKey, result);
                    }
                    break;
                case "UPDATE":
                    if (!SqlCache.TryGetValue(cacheKey, out result))
                    {
                        var list = Mapper.MemberMappers.Keys.Where(f => (!Mapper.PrimaryKeys.Select(m => m.MemberName).Contains(f))).ToArray();
                        result = BuildUpdateSql(Mapper, provider, list);
                        SqlCache.TryAdd(cacheKey, result);
                    }
                    break;
            }
            return result;
        }
        private static Action<IDbCommand, TEntity> GetActionCache(int hashKey, string sql, IOrmProvider provider, bool isPk)
        {
            Action<IDbCommand, TEntity> result;
            if (!ActionCache.TryGetValue(hashKey, out result))
            {
                if (isPk) result = RepositoryHelper.CreateParametersHandler<TEntity>(provider.ParamPrefix, typeof(TEntity), Mapper.PrimaryKeys);
                else
                {
                    var colMappers = Mapper.MemberMappers.Values.Where(p => Regex.IsMatch(sql, @"[?@:]" + p.MemberName + "([^a-z0-9_]+|$)", RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.CultureInvariant));
                    result = RepositoryHelper.CreateParametersHandler<TEntity>(provider.ParamPrefix, typeof(TEntity), colMappers);
                }
                ActionCache.TryAdd(hashKey, result);
            }
            return result;
        }
        private static string BuildCreateSql(EntityMapper mapper, IOrmProvider provider)
        {
            StringBuilder insertBuilder = new StringBuilder();
            insertBuilder.Append("INSERT INTO " + provider.GetQuotedTableName(mapper.TableName) + " (");
            StringBuilder valueBuilder = new StringBuilder();
            valueBuilder.Append(") VALUES(");
            int i = 0;
            foreach (var colMapper in mapper.MemberMappers.Values)
            {
                if (i > 0) insertBuilder.Append(",");
                if (i > 0) valueBuilder.Append(",");
                insertBuilder.Append(provider.GetQuotedColumnName(colMapper.FieldName));
                valueBuilder.Append(provider.ParamPrefix + colMapper.MemberName);
                i++;
            }
            valueBuilder.Append(")");
            return insertBuilder.ToString() + valueBuilder.ToString();
        }
        private static string BuildGetSql(EntityMapper mapper, IOrmProvider provider)
        {
            StringBuilder sqlBuilder = new StringBuilder();
            sqlBuilder.Append("SELECT ");
            StringBuilder whereBuilder = new StringBuilder();
            whereBuilder.Append(" WHERE ");
            int i = 0;
            foreach (var colMapper in mapper.MemberMappers.Values)
            {
                if (i > 0) sqlBuilder.Append(",");
                if (colMapper.IsPrimaryKey)
                {
                    if (i > 0) whereBuilder.Append(" AND ");
                    whereBuilder.Append(GetSetParameterSql(provider, colMapper));
                }
                sqlBuilder.Append(GetAliasParameterSql(provider, colMapper));
                i++;
            }
            sqlBuilder.Append(" FROM " + provider.GetQuotedTableName(mapper.TableName));
            return sqlBuilder.ToString() + whereBuilder.ToString();
        }
        private static string BuildDeleteSql(EntityMapper mapper, IOrmProvider provider)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("DELETE FROM " + provider.GetQuotedTableName(mapper.TableName) + " WHERE ");
            int i = 0;
            foreach (var colMapper in mapper.PrimaryKeys)
            {
                if (i > 0) builder.Append(" AND ");
                builder.Append(GetSetParameterSql(provider, colMapper));
                i++;
            }
            return builder.ToString();
        }
        private static string BuildUpdateSql(EntityMapper mapper, IOrmProvider provider, string[] updateParameters)
        {
            var builder = new StringBuilder("UPDATE " + provider.GetQuotedTableName(mapper.TableName) + " SET ");
            int index = 0;
            foreach (var propName in updateParameters)
            {
                if (index > 0) builder.Append(",");
                builder.Append(GetSetParameterSql(provider, mapper.MemberMappers[propName]));
                index++;
            }
            builder.Append(" WHERE ");
            index = 0;
            foreach (var colMapper in mapper.PrimaryKeys)
            {
                if (index > 0) builder.Append(" AND ");
                builder.Append(GetSetParameterSql(provider, colMapper));
                index++;
            }
            return builder.ToString();
        }
        private static string GetSetParameterSql(IOrmProvider provider, MemberMapper colMapper)
        {
            return provider.GetQuotedColumnName(colMapper.FieldName) + "=" + provider.ParamPrefix + colMapper.MemberName;
        }
        private static string GetAliasParameterSql(IOrmProvider provider, MemberMapper colMapper)
        {
            return provider.GetQuotedColumnName(colMapper.FieldName) + (colMapper.MemberName == colMapper.FieldName ? String.Empty : " AS " + colMapper.MemberName);
        }
    }
}
