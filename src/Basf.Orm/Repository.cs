using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Basf.Repository;
using System.Reflection;

namespace Basf.Orm
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        private static string cacheKey = typeof(Repository<TEntity>).FullName;
        private static string tableName = null;
        private static List<string> columnNames = new List<string>();
        private string ConnKey { get; set; }
        private IDbConnection Connection { get; set; }
        private IDbTransaction Transaction { get; set; }
        private static Dictionary<string, string> sqlCache = new Dictionary<string, string>();
        public Repository(IDbConnection connection, IDbTransaction transaction = null)
        {
            this.Connection = connection;
            this.Transaction = transaction;
            Initialize();
        }
        public TEntity Get(object condition)
        {
            var sqlKey = this.ConnKey + "." + cacheKey + ".Get";
            var sql = "";
            if (!sqlCache.ContainsKey(sqlKey))
            {
                sql = "SELECT ";
                foreach (var colName in columnNames)
                {
                    sql += colName + ",";
                }
                sql = sql.TrimEnd(',');
                sql += " FROM " + tableName + " WHERE ";
            }
            return this.Connection.QueryFirstOrDefault<TEntity>(sql, condition, this.Transaction);
        }

        public int Create(TEntity entity)
        {
            entity.GetHashCode()
            return this.Connection.Execute()
        }
        public TKey CreateSequence<TKey>(string strSequenceCode)
        {
            return default(TKey);
        }
        public int Delete(object objKey)
        {
            return this.Connection.Execute(< TEntity > (objKey, this.Transaction);
        }
        public TEntity Get(object objKey)
        {
            return this.Connection.Read<TEntity>(objKey, this.Transaction);
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
            return this.Connection.ReadAsync<TEntity>(objKey, this.Transaction);
        }
        public Task<int> CreateAsync(TEntity entity)
        {
            return this.Connection.CreateAsync<TEntity>(entity, this.Transaction);
        }
        public Task<int> DeleteAsync(object objKey)
        {
            return this.Connection.DeleteAsync<TEntity>(objKey, this.Transaction);
        }
        public Task<int> UpdateAsync(TEntity entity, object objKey)
        {
            return this.Connection.UpdateAsync<TEntity>(objKey, this.Transaction);
        }


        #region 内部静态方法       
        private static void Initialize()
        {
           string prefix = null;
            string suffix = null;
            if (entityType.IsDefined(typeof(TableAttribute)))
            {
                var attr = type.GetCustomAttribute<TableAttribute>();
                tableName = attr.TableName;
                prefix = attr.ColumnPrefix;
                suffix = attr.ColumnSuffix;
            }
            foreach (var prop in type.GetProperties(BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.Public))
            {
                if (prop.IsDefined(typeof(IgnoreAttribute)))
                {
                    continue;
                }
                columnNames.Add(prefix ?? "" + prop.Name + suffix ?? "");
            }
        }
        #endregion
    }
}
