using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Linq;
using System.Threading.Tasks;

namespace Basf.Repository.Orm
{
    public static class DapperExtensions
    {
        #region 私有静态变量
        private static IOrmProvider ormProvider = null;
        private static ConcurrentDictionary<RuntimeTypeHandle, List<string>> columnNameMap = new ConcurrentDictionary<RuntimeTypeHandle, List<string>>();
        private static ConcurrentDictionary<RuntimeTypeHandle, List<string>> keyNameMap = new ConcurrentDictionary<RuntimeTypeHandle, List<string>>();
        private static ConcurrentDictionary<RuntimeTypeHandle, string> tableNameMap = new ConcurrentDictionary<RuntimeTypeHandle, string>();
        private static readonly object objLocker = new object();
        #endregion

        #region 属性
        public static IOrmProvider OrmProvider
        {
            get
            {
                if (ormProvider == null)
                {
                    lock (objLocker)
                    {
                        if (ormProvider == null)
                        {
                            ormProvider = AppRuntime.Resolve<IOrmProvider>();
                        }
                        Utility.Fail(ormProvider == null, "没有注册IOrmProvider类型的Orm提供者");
                    }
                }
                return ormProvider;
            }
        }
        #endregion

        #region 方法
        public static TEntity Get<TEntity>(this IDbConnection connection, object condition, IDbTransaction transaction = null) where TEntity : class
        {
            string tableName = OrmProvider.TableNameFor(tableNameMap[typeof(TEntity).TypeHandle]);
            List<string> whereList = GetColumnNameList<TEntity>(condition);
            string wheres = String.Join(" AND ", whereList.Select(p => p + "=" + OrmProvider.ParamString + p));
            string sql = String.Format("SELECT * FROM {0} WHERE {1}", tableName, wheres);
            return connection.Query<TEntity>(sql, condition, transaction, true, OrmProvider.CommandTimeout).FirstOrDefault();
        }
        public static Task<TEntity> GetAsync<TEntity>(this IDbConnection connection, object condition, IDbTransaction transaction = null) where TEntity : class
        {
            string tableName = OrmProvider.TableNameFor(tableNameMap[typeof(TEntity).TypeHandle]);
            List<string> whereList = GetColumnNameList<TEntity>(condition);
            string wheres = String.Join(" AND ", whereList.Select(p => p + "=" + OrmProvider.ParamString + p));
            string sql = String.Format("SELECT * FROM {0} WHERE {1}", tableName, wheres);
            return connection.GetAsync<TEntity>(sql, condition, transaction, OrmProvider.CommandTimeout);
        }
        public static int Insert<TEntity>(this IDbConnection connection, dynamic data, IDbTransaction transaction = null) where TEntity : class
        {
            object obj = data as object;
            List<string> columnList = GetColumnNameList<TEntity>(obj);
            string columns = String.Join(",", columnList);
            string values = String.Join(",", columnList.Select(p => OrmProvider.ParamString + p));
            string tableName = OrmProvider.TableNameFor(tableNameMap[typeof(TEntity).TypeHandle]);
            string sql = String.Format("INSERT INTO {0}({1}) VALUES ({2}))", tableName, columns, values);
            return connection.Execute(sql, obj, transaction, OrmProvider.CommandTimeout, CommandType.Text);
        }
        public static Task<int> InsertAsync<TEntity>(this IDbConnection connection, dynamic data, IDbTransaction transaction = null) where TEntity : class
        {
            object obj = data as object;
            List<string> columnList = GetColumnNameList<TEntity>(obj);
            string columns = String.Join(",", columnList);
            string values = String.Join(",", columnList.Select(p => OrmProvider.ParamString + p));
            string tableName = OrmProvider.TableNameFor(tableNameMap[typeof(TEntity).TypeHandle]);
            string sql = String.Format("INSERT INTO {0}({1}) VALUES ({2}))", tableName, columns, values);
            return connection.ExecuteAsync(sql, obj, transaction, OrmProvider.CommandTimeout);
        }
        public static int Update<TEntity>(this IDbConnection connection, dynamic data, dynamic condition, IDbTransaction transaction = null) where TEntity : class
        {
            object obj = data as object;
            List<string> columnList = GetColumnNameList<TEntity>(obj);
            string columns = String.Join(",", columnList.Select(p => p + "=" + OrmProvider.ParamString + p));
            List<string> whereList = GetColumnNameList<TEntity>(condition as object);
            string wheres = String.Join(" AND ", whereList.Select(p => p + "=" + OrmProvider.ParamString + "p" + p));
            string tableName = OrmProvider.TableNameFor(tableNameMap[typeof(TEntity).TypeHandle]);
            string sql = String.Format("UPDATE {0} SET {1} WHERE {2}", tableName, columns, wheres);
            DynamicParameters parameters = new DynamicParameters(condition);
            parameters.ParameterNames.AsList().ForEach(p => p = "p" + p);
            parameters.AddDynamicParams(data);
            return connection.Execute(sql, parameters, transaction, OrmProvider.CommandTimeout);
        }
        public static Task<int> UpdateAsync<TEntity>(this IDbConnection connection, dynamic data, dynamic condition, IDbTransaction transaction = null) where TEntity : class
        {
            object obj = data as object;
            List<string> columnList = GetColumnNameList<TEntity>(obj);
            string columns = String.Join(",", columnList.Select(p => p + "=" + OrmProvider.ParamString + p));
            List<string> whereList = GetColumnNameList<TEntity>(condition as object);
            string wheres = String.Join(" AND ", whereList.Select(p => p + "=" + OrmProvider.ParamString + "p" + p));
            string tableName = OrmProvider.TableNameFor(tableNameMap[typeof(TEntity).TypeHandle]);
            string sql = String.Format("UPDATE {0} SET {1} WHERE {2}", tableName, columns, wheres);
            DynamicParameters parameters = new DynamicParameters(condition);
            parameters.ParameterNames.AsList().ForEach(p => p = "p" + p);
            parameters.AddDynamicParams(data);
            return connection.ExecuteAsync(sql, parameters, transaction, OrmProvider.CommandTimeout);
        }
        public static int Delete<TEntity>(this IDbConnection connection, dynamic condition, IDbTransaction transaction = null) where TEntity : class
        {
            string tableName = OrmProvider.TableNameFor(tableNameMap[typeof(TEntity).TypeHandle]);
            List<string> whereList = GetColumnNameList<TEntity>(condition as object);
            string wheres = String.Join(" AND ", whereList.Select(p => p + "=" + OrmProvider.ParamString + p));
            string sql = String.Format("DELETE FROM {0} WHERE {1}", tableName, wheres);
            return connection.Execute(sql, new DynamicParameters(condition), transaction, OrmProvider.CommandTimeout);
        }
        public static Task<int> DeleteAsync<TEntity>(this IDbConnection connection, dynamic condition, IDbTransaction transaction = null) where TEntity : class
        {
            string tableName = OrmProvider.TableNameFor(tableNameMap[typeof(TEntity).TypeHandle]);
            List<string> whereList = GetColumnNameList<TEntity>(condition as object);
            string wheres = String.Join(" AND ", whereList.Select(p => p + "=" + OrmProvider.ParamString + p));
            string sql = String.Format("DELETE FROM {0} WHERE {1}", tableName, wheres);
            return connection.ExecuteAsync(sql, new DynamicParameters(condition), transaction, OrmProvider.CommandTimeout);
        }
        public static int Count<TEntity>(this IDbConnection connection, object condition, IDbTransaction transaction = null) where TEntity : class
        {
            string tableName = OrmProvider.TableNameFor(tableNameMap[typeof(TEntity).TypeHandle]);
            List<string> whereList = GetColumnNameList<TEntity>(condition);
            string wheres = String.Join(" AND ", whereList.Select(p => p + "=" + OrmProvider.ParamString + p));
            string sql = String.Format("SELECT COUNT(*) FROM {0} WHERE {1}", tableName, wheres);
            return connection.ExecuteScalar<int>(sql, condition, transaction, OrmProvider.CommandTimeout);
        }
        public static IEnumerable<TEntity> QueryList<TEntity>(this IDbConnection connection, object condition, IDbTransaction transaction = null) where TEntity : class
        {
            string tableName = OrmProvider.TableNameFor(tableNameMap[typeof(TEntity).TypeHandle]);
            List<string> whereList = GetColumnNameList<TEntity>(condition);
            string wheres = String.Join(" AND ", whereList.Select(p => p + "=" + OrmProvider.ParamString + p));
            string sql = String.Format("SELECT * FROM {0} WHERE {1}", tableName, wheres);
            return connection.Query<TEntity>(sql, condition, transaction, true, OrmProvider.CommandTimeout);
        }
        public static Task<IEnumerable<TEntity>> QueryListAsync<TEntity>(this IDbConnection connection, object condition, IDbTransaction transaction = null) where TEntity : class
        {
            string tableName = OrmProvider.TableNameFor(tableNameMap[typeof(TEntity).TypeHandle]);
            List<string> whereList = GetColumnNameList<TEntity>(condition);
            string wheres = String.Join(" AND ", whereList.Select(p => p + "=" + OrmProvider.ParamString + p));
            string sql = String.Format("SELECT * FROM {0} WHERE {1}", tableName, wheres);
            return connection.QueryAsync<TEntity>(sql, condition, transaction, OrmProvider.CommandTimeout);
        }
        #endregion

        #region 内部静态方法
        internal static List<string> GetColumnNameList<TEntity>() where TEntity : class
        {
            Type type = typeof(TEntity);
            if (!tableNameMap.ContainsKey(type.TypeHandle))
            {
                Initialize(type);
            }
            return columnNameMap[type.TypeHandle];
        }
        internal static void Initialize(Type type)
        {
            string tableName = type.Name;

            if (!type.IsDefined(typeof(TableAttribute)))
            {
                tableName = type.GetCustomAttribute<TableAttribute>().TableName;
            }
            else
            {
                tableName = ormProvider.TableNameFor(tableName);
            }
            List<string> columnNames = new List<string>();
            List<string> keyNames = new List<string>();
            foreach (var prop in type.GetProperties(BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.Public))
            {
                if (!prop.IsDefined(typeof(IgnoreAttribute)))
                {
                    columnNames.Add(ormProvider.ColumnNameFor(prop.Name));
                }
                if (!prop.IsDefined(typeof(PrimaryKeyAttribute)))
                {
                    keyNames.Add(ormProvider.ColumnNameFor(prop.Name));
                }
            }
            columnNameMap[type.TypeHandle] = columnNames;
            keyNameMap[type.TypeHandle] = keyNames;
        }
        internal static List<string> GetColumnNameList<TEntity>(object objData) where TEntity : class
        {
            List<string> columnList = GetColumnNameList<TEntity>();
            Type objType = objData.GetType();
            if (typeof(TEntity) == objType)
            {
                return columnList;
            }
            string columnName = null;
            List<string> columnNames = new List<string>();
            foreach (var prop in objType.GetProperties(BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.Public))
            {
                if (prop.IsDefined(typeof(IgnoreAttribute)))
                {
                    continue;
                }
                columnName = ormProvider.ColumnNameFor(prop.Name);
                if (!columnList.Contains(columnName))
                {
                    continue;
                }
                columnNames.Add(columnName);
            }
            return columnNames;
        }       
        #endregion
    }
}