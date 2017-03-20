using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.Threading.Tasks;

namespace Basf.Repository
{
    public class Repository : IRepository
    {
        protected DbConnection connection = null;
        protected DbTransaction Transaction { get; set; }
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
        public TEntity QueryFirst<TEntity>(string sql, object objParameters = null, CommandType cmdType = CommandType.Text)
        {
            return this.QueryFirstImpl<TEntity>(typeof(TEntity), sql, cmdType, objParameters);
        }
        public List<TEntity> Query<TEntity>(string sql, object objParameters = null, CommandType cmdType = CommandType.Text)
        {
            return this.QueryImpl<TEntity>(typeof(TEntity), sql, cmdType, objParameters);
        }
        public PagedList<TEntity> QueryPage<TEntity>(string sql, int pageIndex, int? pageSize, string orderBy = null, object objParameters = null, CommandType cmdType = CommandType.Text)
        {
            return this.QueryPageImpl<TEntity>(typeof(TEntity), sql, cmdType, objParameters);
        }
        public int ExecSql(string sql, object objParameters = null, CommandType cmdType = CommandType.Text)
        {
            return this.ExecSqlImpl(sql, cmdType, objParameters);
        }

        public TEntity QueryFirst<TEntity>(Action<SqlBuilder> builder, object objParameters = null, CommandType cmdType = CommandType.Text)
        {
            return this.QueryFirstImpl<TEntity>(typeof(TEntity), builder, cmdType, objParameters);
        }
        public List<TEntity> Query<TEntity>(Action<SqlBuilder> builder, object objParameters = null, CommandType cmdType = CommandType.Text)
        {
            return this.QueryImpl<TEntity>(typeof(TEntity), builder, cmdType, objParameters);
        }
        public int ExecSql(Action<SqlBuilder> builder, object objParameters = null, CommandType cmdType = CommandType.Text)
        {
            return this.ExecSqlImpl(builder, cmdType, objParameters);
        }

        public async Task<TEntity> QueryFirstAsync<TEntity>(string sql, object objParameters = null, CommandType cmdType = CommandType.Text)
        {
            return await this.QueryFirstImplAsync<TEntity>(typeof(TEntity), sql, cmdType, objParameters);
        }
        public async Task<List<TEntity>> QueryAsync<TEntity>(string sql, object objParameters = null, CommandType cmdType = CommandType.Text)
        {
            return await this.QueryImplAsync<TEntity>(typeof(TEntity), sql, cmdType, objParameters);
        }
        public async Task<PagedList<TEntity>> QueryPageAsync<TEntity>(string sql, int pageIndex, int? pageSize, string orderBy = null, object objParameters = null, CommandType cmdType = CommandType.Text)
        {
            return await this.QueryPageImplAsync<TEntity>(typeof(TEntity), sql, cmdType, objParameters);
        }
        public async Task<int> ExecSqlAsync(string sql, object objParameters = null, CommandType cmdType = CommandType.Text)
        {
            return await this.ExecSqlImplAsync(sql, cmdType, objParameters);
        }

        public async Task<TEntity> QueryFirstAsync<TEntity>(Action<SqlBuilder> builder, object objParameters = null, CommandType cmdType = CommandType.Text)
        {
            return await this.QueryFirstImplAsync<TEntity>(typeof(TEntity), builder, cmdType, objParameters);
        }
        public async Task<List<TEntity>> QueryAsync<TEntity>(Action<SqlBuilder> builder, object objParameters = null, CommandType cmdType = CommandType.Text)
        {
            return await this.QueryImplAsync<TEntity>(typeof(TEntity), builder, cmdType, objParameters);
        }
        public async Task<int> ExecSqlAsync(Action<SqlBuilder> builder, object objParameters = null, CommandType cmdType = CommandType.Text)
        {
            return await this.ExecSqlImplAsync(builder, cmdType, objParameters);
        }

        public void Dispose()
        {
            if (this.Transaction != null) this.Transaction.Dispose();
            if (this.connection != null) this.connection.Dispose();
        }
        private TEntity QueryFirstImpl<TEntity>(Type entityType, string sql, CommandType cmdType, object objParameters = null)
        {
            DbCommand command = null;
            string propName = String.Empty;
            DbDataReader reader = null;
            TEntity result = default(TEntity);
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
                        var paramAction = RepositoryHelper.GetActionCache(hashKey, sql, objParameters.GetType(), this.Provider);
                        paramAction(command, objParameters);
                    }
                    conn.Open();
                    reader = command.ExecuteReader(CommandBehavior.CloseConnection | CommandBehavior.SequentialAccess | CommandBehavior.SingleResult | CommandBehavior.SingleRow);
                    var func = RepositoryHelper.GetReader(hashKey, entityType, reader);
                    while (reader.Read())
                    {
                        object funcResult = func?.Invoke(reader);
                        if (funcResult == null || funcResult is TEntity) result = (TEntity)funcResult;
                        else result = (TEntity)Convert.ChangeType(funcResult, entityType, CultureInfo.InvariantCulture);
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
                    var paramAction = RepositoryHelper.GetActionCache(hashKey, sql, objParameters.GetType(), this.Provider);
                    paramAction(command, objParameters);
                }
                this.Open();
                reader = command.ExecuteReader(CommandBehavior.SequentialAccess | CommandBehavior.SingleResult | CommandBehavior.SingleRow);
                var func = RepositoryHelper.GetReader(hashKey, entityType, reader);
                while (reader.Read())
                {
                    object funcResult = func?.Invoke(reader);
                    if (funcResult == null || funcResult is TEntity) result = (TEntity)funcResult;
                    else result = (TEntity)Convert.ChangeType(funcResult, entityType, CultureInfo.InvariantCulture);
                }
                while (reader.NextResult()) { }
                reader.Dispose();
                reader = null;
            }
            return result;
        }
        private List<TEntity> QueryImpl<TEntity>(Type entityType, string sql, CommandType cmdType, object objParameters = null)
        {
            DbCommand command = null;
            string propName = String.Empty;
            DbDataReader reader = null;
            List<TEntity> result = new List<TEntity>();
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
                        var paramAction = RepositoryHelper.GetActionCache(hashKey, sql, objParameters.GetType(), this.Provider);
                        paramAction(command, objParameters);
                    }
                    conn.Open();
                    reader = command.ExecuteReader(CommandBehavior.CloseConnection | CommandBehavior.SequentialAccess | CommandBehavior.SingleResult | CommandBehavior.SingleRow);
                    var func = RepositoryHelper.GetReader(hashKey, entityType, reader);
                    while (reader.Read())
                    {
                        object funcResult = func?.Invoke(reader);
                        if (funcResult == null) continue;
                        if (funcResult is TEntity) result.Add((TEntity)funcResult);
                        else result.Add((TEntity)Convert.ChangeType(funcResult, entityType, CultureInfo.InvariantCulture));
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
                    var paramAction = RepositoryHelper.GetActionCache(hashKey, sql, objParameters.GetType(), this.Provider);
                    paramAction(command, objParameters);
                }
                this.Open();
                reader = command.ExecuteReader(CommandBehavior.SequentialAccess);
                var func = RepositoryHelper.GetReader(hashKey, entityType, reader);
                while (reader.Read())
                {
                    object funcResult = func?.Invoke(reader);
                    if (funcResult == null) continue;
                    if (funcResult is TEntity) result.Add((TEntity)funcResult);
                    else result.Add((TEntity)Convert.ChangeType(funcResult, entityType, CultureInfo.InvariantCulture));
                }
                while (reader.NextResult()) { }
                reader.Dispose();
                reader = null;
            }
            return result;
        }
        private PagedList<TEntity> QueryPageImpl<TEntity>(Type entityType, string sql, CommandType cmdType, object objParameters = null)
        {
            DbCommand command = null;
            string propName = String.Empty;
            DbDataReader reader = null;
            PagedList<TEntity> result = new PagedList<TEntity>();
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
                        var paramAction = RepositoryHelper.GetActionCache(hashKey, sql, objParameters.GetType(), this.Provider);
                        paramAction(command, objParameters);
                    }
                    conn.Open();
                    reader = command.ExecuteReader(CommandBehavior.CloseConnection | CommandBehavior.SequentialAccess);
                    var func = RepositoryHelper.GetReader(hashKey, entityType, reader);
                    while (reader.Read())
                    {
                        object funcResult = func?.Invoke(reader);
                        if (funcResult == null) continue;
                        if (funcResult is TEntity) result.Add((TEntity)funcResult);
                        else result.Add((TEntity)Convert.ChangeType(funcResult, entityType, CultureInfo.InvariantCulture));
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
                    var paramAction = RepositoryHelper.GetActionCache(hashKey, sql, objParameters.GetType(), this.Provider);
                    paramAction(command, objParameters);
                }
                this.Open();
                reader = command.ExecuteReader(CommandBehavior.SequentialAccess);
                var func = RepositoryHelper.GetReader(hashKey, entityType, reader);
                while (reader.Read())
                {
                    object funcResult = func?.Invoke(reader);
                    if (funcResult == null) continue;
                    if (funcResult is TEntity) result.Add((TEntity)funcResult);
                    else result.Add((TEntity)Convert.ChangeType(funcResult, entityType, CultureInfo.InvariantCulture));
                }
                while (reader.NextResult()) { }
                reader.Dispose();
                reader = null;
            }
            return result;
        }
        private int ExecSqlImpl(string sql, CommandType cmdType = CommandType.Text, object objParameters = null)
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
                        var paramAction = RepositoryHelper.GetActionCache(hashKey, sql, objParameters.GetType(), this.Provider);
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
                    var paramAction = RepositoryHelper.GetActionCache(hashKey, sql, objParameters.GetType(), this.Provider);
                    paramAction(command, objParameters);
                }
                this.Open();
                result = command.ExecuteNonQuery();
            }
            return result;
        }
        private async Task<TEntity> QueryFirstImplAsync<TEntity>(Type entityType, string sql, CommandType cmdType, object objParameters = null)
        {
            DbCommand command = null;
            string propName = String.Empty;
            DbDataReader reader = null;
            TEntity result = default(TEntity);
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
                        var paramAction = RepositoryHelper.GetActionCache(hashKey, sql, objParameters.GetType(), this.Provider);
                        paramAction(command, objParameters);
                    }
                    conn.Open();
                    reader = await command.ExecuteReaderAsync(CommandBehavior.CloseConnection | CommandBehavior.SequentialAccess | CommandBehavior.SingleResult | CommandBehavior.SingleRow);
                    var func = RepositoryHelper.GetReader(hashKey, entityType, reader);
                    while (reader.Read())
                    {
                        object funcResult = func?.Invoke(reader);
                        if (funcResult == null || funcResult is TEntity) result = (TEntity)funcResult;
                        else result = (TEntity)Convert.ChangeType(funcResult, entityType, CultureInfo.InvariantCulture);
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
                    var paramAction = RepositoryHelper.GetActionCache(hashKey, sql, objParameters.GetType(), this.Provider);
                    paramAction(command, objParameters);
                }
                this.Open();
                reader = await command.ExecuteReaderAsync(CommandBehavior.SequentialAccess | CommandBehavior.SingleResult | CommandBehavior.SingleRow);
                var func = RepositoryHelper.GetReader(hashKey, entityType, reader);
                while (reader.Read())
                {
                    object funcResult = func?.Invoke(reader);
                    if (funcResult == null || funcResult is TEntity) result = (TEntity)funcResult;
                    else result = (TEntity)Convert.ChangeType(funcResult, entityType, CultureInfo.InvariantCulture);
                }
                while (reader.NextResult()) { }
                reader.Dispose();
                reader = null;
            }
            return result;
        }
        private async Task<List<TEntity>> QueryImplAsync<TEntity>(Type entityType, string sql, CommandType cmdType, object objParameters = null)
        {
            DbCommand command = null;
            string propName = String.Empty;
            DbDataReader reader = null;
            List<TEntity> result = new List<TEntity>();
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
                        var paramAction = RepositoryHelper.GetActionCache(hashKey, sql, objParameters.GetType(), this.Provider);
                        paramAction(command, objParameters);
                    }
                    conn.Open();
                    reader = await command.ExecuteReaderAsync(CommandBehavior.CloseConnection | CommandBehavior.SequentialAccess);
                    var func = RepositoryHelper.GetReader(hashKey, entityType, reader);
                    while (reader.Read())
                    {
                        object funcResult = func?.Invoke(reader);
                        if (funcResult == null) continue;
                        if (funcResult is TEntity) result.Add((TEntity)funcResult);
                        else result.Add((TEntity)Convert.ChangeType(funcResult, entityType, CultureInfo.InvariantCulture));
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
                    var paramAction = RepositoryHelper.GetActionCache(hashKey, sql, objParameters.GetType(), this.Provider);
                    paramAction(command, objParameters);
                }
                this.Open();
                reader = await command.ExecuteReaderAsync(CommandBehavior.SequentialAccess);
                var func = RepositoryHelper.GetReader(hashKey, entityType, reader);
                while (reader.Read())
                {
                    object funcResult = func?.Invoke(reader);
                    if (funcResult == null) continue;
                    if (funcResult is TEntity) result.Add((TEntity)funcResult);
                    else result.Add((TEntity)Convert.ChangeType(funcResult, entityType, CultureInfo.InvariantCulture));
                }
                while (reader.NextResult()) { }
                reader.Dispose();
                reader = null;
            }
            return result;
        }
        private async Task<PagedList<TEntity>> QueryPageImplAsync<TEntity>(Type entityType, string sql, CommandType cmdType, object objParameters = null)
        {
            DbCommand command = null;
            string propName = String.Empty;
            DbDataReader reader = null;
            PagedList<TEntity> result = new PagedList<TEntity>();
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
                        var paramAction = RepositoryHelper.GetActionCache(hashKey, sql, objParameters.GetType(), this.Provider);
                        paramAction(command, objParameters);
                    }
                    conn.Open();
                    reader = await command.ExecuteReaderAsync(CommandBehavior.CloseConnection | CommandBehavior.SequentialAccess);
                    var func = RepositoryHelper.GetReader(hashKey, entityType, reader);
                    while (reader.Read())
                    {
                        object funcResult = func?.Invoke(reader);
                        if (funcResult == null) continue;
                        if (funcResult is TEntity) result.Add((TEntity)funcResult);
                        else result.Add((TEntity)Convert.ChangeType(funcResult, entityType, CultureInfo.InvariantCulture));
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
                    var paramAction = RepositoryHelper.GetActionCache(hashKey, sql, objParameters.GetType(), this.Provider);
                    paramAction(command, objParameters);
                }
                this.Open();
                reader = await command.ExecuteReaderAsync(CommandBehavior.SequentialAccess);
                var func = RepositoryHelper.GetReader(hashKey, entityType, reader);
                while (reader.Read())
                {
                    object funcResult = func?.Invoke(reader);
                    if (funcResult == null) continue;
                    if (funcResult is TEntity) result.Add((TEntity)funcResult);
                    else result.Add((TEntity)Convert.ChangeType(funcResult, entityType, CultureInfo.InvariantCulture));
                }
                while (reader.NextResult()) { }
                reader.Dispose();
                reader = null;
            }
            return result;
        }
        private async Task<int> ExecSqlImplAsync(string sql, CommandType cmdType = CommandType.Text, object objParameters = null)
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
                        var paramAction = RepositoryHelper.GetActionCache(hashKey, sql, objParameters.GetType(), this.Provider);
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
                    var paramAction = RepositoryHelper.GetActionCache(hashKey, sql, objParameters.GetType(), this.Provider);
                    paramAction(command, objParameters);
                }
                this.Open();
                result = await command.ExecuteNonQueryAsync();
            }
            return result;
        }

        private TEntity QueryFirstImpl<TEntity>(Type entityType, Action<SqlBuilder> builder, CommandType cmdType, object objParameters = null)
        {
            DbCommand command = null;
            string propName = String.Empty;
            DbDataReader reader = null;
            TEntity result = default(TEntity);
            SqlBuilder sqlBuidler = new SqlBuilder(this.Provider);
            builder.Invoke(sqlBuidler);
            var sql = sqlBuidler.BuildSql();
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
                        var paramAction = RepositoryHelper.GetActionCache(hashKey, sql, objParameters.GetType(), this.Provider);
                        paramAction(command, objParameters);
                    }
                    conn.Open();
                    reader = command.ExecuteReader(CommandBehavior.CloseConnection | CommandBehavior.SequentialAccess | CommandBehavior.SingleResult | CommandBehavior.SingleRow);
                    var func = RepositoryHelper.GetReader(hashKey, entityType, reader);
                    while (reader.Read())
                    {
                        object funcResult = func?.Invoke(reader);
                        if (funcResult == null || funcResult is TEntity) result = (TEntity)funcResult;
                        else result = (TEntity)Convert.ChangeType(funcResult, entityType, CultureInfo.InvariantCulture);
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
                    var paramAction = RepositoryHelper.GetActionCache(hashKey, sql, objParameters.GetType(), this.Provider);
                    paramAction(command, objParameters);
                }
                this.Open();
                reader = command.ExecuteReader(CommandBehavior.SequentialAccess | CommandBehavior.SingleResult | CommandBehavior.SingleRow);
                var func = RepositoryHelper.GetReader(hashKey, entityType, reader);
                while (reader.Read())
                {
                    object funcResult = func?.Invoke(reader);
                    if (funcResult == null || funcResult is TEntity) result = (TEntity)funcResult;
                    else result = (TEntity)Convert.ChangeType(funcResult, entityType, CultureInfo.InvariantCulture);
                }
                while (reader.NextResult()) { }
                reader.Dispose();
                reader = null;
            }
            return result;
        }
        private List<TEntity> QueryImpl<TEntity>(Type entityType, Action<SqlBuilder> builder, CommandType cmdType, object objParameters = null)
        {
            DbCommand command = null;
            string propName = String.Empty;
            DbDataReader reader = null;
            List<TEntity> result = new List<TEntity>();
            SqlBuilder sqlBuidler = new SqlBuilder(this.Provider);
            builder.Invoke(sqlBuidler);
            var sql = sqlBuidler.BuildSql();
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
                        var paramAction = RepositoryHelper.GetActionCache(hashKey, sql, objParameters.GetType(), this.Provider);
                        paramAction(command, objParameters);
                    }
                    conn.Open();
                    reader = command.ExecuteReader(CommandBehavior.CloseConnection | CommandBehavior.SequentialAccess | CommandBehavior.SingleResult | CommandBehavior.SingleRow);
                    var func = RepositoryHelper.GetReader(hashKey, entityType, reader);
                    while (reader.Read())
                    {
                        object funcResult = func?.Invoke(reader);
                        if (funcResult == null) continue;
                        if (funcResult is TEntity) result.Add((TEntity)funcResult);
                        else result.Add((TEntity)Convert.ChangeType(funcResult, entityType, CultureInfo.InvariantCulture));
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
                    var paramAction = RepositoryHelper.GetActionCache(hashKey, sql, objParameters.GetType(), this.Provider);
                    paramAction(command, objParameters);
                }
                this.Open();
                reader = command.ExecuteReader(CommandBehavior.SequentialAccess);
                var func = RepositoryHelper.GetReader(hashKey, entityType, reader);
                while (reader.Read())
                {
                    object funcResult = func?.Invoke(reader);
                    if (funcResult == null) continue;
                    if (funcResult is TEntity) result.Add((TEntity)funcResult);
                    else result.Add((TEntity)Convert.ChangeType(funcResult, entityType, CultureInfo.InvariantCulture));
                }
                while (reader.NextResult()) { }
                reader.Dispose();
                reader = null;
            }
            return result;
        }
        private int ExecSqlImpl(Action<SqlBuilder> builder, CommandType cmdType = CommandType.Text, object objParameters = null)
        {
            DbCommand command = null;
            int result = 0;
            SqlBuilder sqlBuidler = new SqlBuilder(this.Provider);
            builder.Invoke(sqlBuidler);
            var sql = sqlBuidler.BuildSql();
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
                        var paramAction = RepositoryHelper.GetActionCache(hashKey, sql, objParameters.GetType(), this.Provider);
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
                    var paramAction = RepositoryHelper.GetActionCache(hashKey, sql, objParameters.GetType(), this.Provider);
                    paramAction(command, objParameters);
                }
                this.Open();
                result = command.ExecuteNonQuery();
            }
            return result;
        }
        private async Task<TEntity> QueryFirstImplAsync<TEntity>(Type entityType, Action<SqlBuilder> builder, CommandType cmdType, object objParameters = null)
        {
            DbCommand command = null;
            string propName = String.Empty;
            DbDataReader reader = null;
            TEntity result = default(TEntity);
            SqlBuilder sqlBuidler = new SqlBuilder(this.Provider);
            builder.Invoke(sqlBuidler);
            var sql = sqlBuidler.BuildSql();
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
                        var paramAction = RepositoryHelper.GetActionCache(hashKey, sql, objParameters.GetType(), this.Provider);
                        paramAction(command, objParameters);
                    }
                    conn.Open();
                    reader = await command.ExecuteReaderAsync(CommandBehavior.CloseConnection | CommandBehavior.SequentialAccess | CommandBehavior.SingleResult | CommandBehavior.SingleRow);
                    var func = RepositoryHelper.GetReader(hashKey, entityType, reader);
                    while (reader.Read())
                    {
                        object funcResult = func?.Invoke(reader);
                        if (funcResult == null || funcResult is TEntity) result = (TEntity)funcResult;
                        else result = (TEntity)Convert.ChangeType(funcResult, entityType, CultureInfo.InvariantCulture);
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
                    var paramAction = RepositoryHelper.GetActionCache(hashKey, sql, objParameters.GetType(), this.Provider);
                    paramAction(command, objParameters);
                }
                this.Open();
                reader = await command.ExecuteReaderAsync(CommandBehavior.SequentialAccess | CommandBehavior.SingleResult | CommandBehavior.SingleRow);
                var func = RepositoryHelper.GetReader(hashKey, entityType, reader);
                while (reader.Read())
                {
                    object funcResult = func?.Invoke(reader);
                    if (funcResult == null || funcResult is TEntity) result = (TEntity)funcResult;
                    else result = (TEntity)Convert.ChangeType(funcResult, entityType, CultureInfo.InvariantCulture);
                }
                while (reader.NextResult()) { }
                reader.Dispose();
                reader = null;
            }
            return result;
        }
        private async Task<List<TEntity>> QueryImplAsync<TEntity>(Type entityType, Action<SqlBuilder> builder, CommandType cmdType, object objParameters = null)
        {
            DbCommand command = null;
            string propName = String.Empty;
            DbDataReader reader = null;
            List<TEntity> result = new List<TEntity>();
            SqlBuilder sqlBuidler = new SqlBuilder(this.Provider);
            builder.Invoke(sqlBuidler);
            var sql = sqlBuidler.BuildSql();
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
                        var paramAction = RepositoryHelper.GetActionCache(hashKey, sql, objParameters.GetType(), this.Provider);
                        paramAction(command, objParameters);
                    }
                    conn.Open();
                    reader = await command.ExecuteReaderAsync(CommandBehavior.CloseConnection | CommandBehavior.SequentialAccess);
                    var func = RepositoryHelper.GetReader(hashKey, entityType, reader);
                    while (reader.Read())
                    {
                        object funcResult = func?.Invoke(reader);
                        if (funcResult == null) continue;
                        if (funcResult is TEntity) result.Add((TEntity)funcResult);
                        else result.Add((TEntity)Convert.ChangeType(funcResult, entityType, CultureInfo.InvariantCulture));
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
                    var paramAction = RepositoryHelper.GetActionCache(hashKey, sql, objParameters.GetType(), this.Provider);
                    paramAction(command, objParameters);
                }
                this.Open();
                reader = await command.ExecuteReaderAsync(CommandBehavior.SequentialAccess);
                var func = RepositoryHelper.GetReader(hashKey, entityType, reader);
                while (reader.Read())
                {
                    object funcResult = func?.Invoke(reader);
                    if (funcResult == null) continue;
                    if (funcResult is TEntity) result.Add((TEntity)funcResult);
                    else result.Add((TEntity)Convert.ChangeType(funcResult, entityType, CultureInfo.InvariantCulture));
                }
                while (reader.NextResult()) { }
                reader.Dispose();
                reader = null;
            }
            return result;
        }
        private async Task<int> ExecSqlImplAsync(Action<SqlBuilder> builder, CommandType cmdType = CommandType.Text, object objParameters = null)
        {
            DbCommand command = null;
            int result = 0;
            SqlBuilder sqlBuidler = new SqlBuilder(this.Provider);
            builder.Invoke(sqlBuidler);
            var sql = sqlBuidler.BuildSql();
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
                        var paramAction = RepositoryHelper.GetActionCache(hashKey, sql, objParameters.GetType(), this.Provider);
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
                    var paramAction = RepositoryHelper.GetActionCache(hashKey, sql, objParameters.GetType(), this.Provider);
                    paramAction(command, objParameters);
                }
                this.Open();
                result = await command.ExecuteNonQueryAsync();
            }
            return result;
        }

        private void Open()
        {
            if (this.connection.State == ConnectionState.Broken) this.connection.Close();
            if (this.connection.State == ConnectionState.Closed) this.connection.Open();
        }
    }
}
