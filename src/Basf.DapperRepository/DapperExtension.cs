using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Basf.Orm
{
    public class DapperExtension
    {
        static ConcurrentDictionary<RuntimeTypeHandle, List<string>> tableMap = new ConcurrentDictionary<RuntimeTypeHandle, List<string>>();
        static ConcurrentDictionary<RuntimeTypeHandle, List<string>> tableKeyMap = new ConcurrentDictionary<RuntimeTypeHandle, List<string>>();
        static ConcurrentDictionary<RuntimeTypeHandle, string> tableNameMap = new ConcurrentDictionary<RuntimeTypeHandle, string>();
        private string DetermineTableName<T>(string likelyTableName)
        {
            string name;

            if (!tableNameMap.TryGetValue(typeof(T), out name))
            {
                name = likelyTableName;
                if (!TableExists(name))
                {
                    name = "[" + typeof(T).Name + "]";
                }

                tableNameMap[typeof(T)] = name;
            }
            return name;
        }
        internal static List<string> GetColumnNames<TEntity>() where TEntity : class
        {
            List<string> columnNames;
            List<string> keyNames;
            Type type = typeof(TEntity);
            if (!tableMap.TryGetValue(type.TypeHandle, out columnNames))
            {
                columnNames = new List<string>();
                foreach (var prop in type.GetProperties(BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.SetProperty))
                {
                    var attribs = prop.GetCustomAttributes(typeof(IgnorePropertyAttribute), true);
                    var attr = attribs.FirstOrDefault() as IgnorePropertyAttribute;
                    if (attr == null || (attr != null && !attr.Value))
                    {
                        columnNames.Add(prop.Name);
                    }
                      attribs = prop.GetCustomAttributes(typeof(KeyPropertyAttribute), true);
                      attr = attribs.FirstOrDefault() as IgnorePropertyAttribute;
                    if (attr == null || (attr != null && !attr.Value))
                    {
                        columnNames.Add(prop.Name);
                    }
                }
                tableMap[type.TypeHandle] = columnNames;
            }
            if (!tableKeyMap.TryGetValue(type.TypeHandle, out keyNames))
            {
                columnNames = new List<string>();
                foreach (var prop in type.GetProperties(BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.SetProperty))
                {
                    var attribs = prop.GetCustomAttributes(typeof(IgnorePropertyAttribute), true);
                    var attr = attribs.FirstOrDefault() as IgnorePropertyAttribute;
                    if (attr == null || (attr != null && !attr.Value))
                    {
                        columnNames.Add(prop.Name);
                    }
                }
                tableMap[type.TypeHandle] = columnNames;
            }
            return columnNames;
        }
        internal static List<string> GetColumnNames(object objEntity)
        {
            if (objEntity is DynamicParameters)
            {
                return (objEntity as DynamicParameters).ParameterNames.ToList();
            }
            List<string> columnNames;
            if (!tableKeyMap.TryGetValue(objEntity.GetType().TypeHandle, out columnNames))
            {
                columnNames = new List<string>();
                foreach (var prop in objEntity.GetType().GetProperties(BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.SetProperty))
                {
                    var attribs = prop.GetCustomAttributes(typeof(IgnorePropertyAttribute), true);
                    var attr = attribs.FirstOrDefault() as IgnorePropertyAttribute;
                    if (attr == null || (attr != null && !attr.Value))
                    {
                        columnNames.Add(prop.Name);
                    }
                }
                tableMap[objEntity.GetType().TypeHandle] = columnNames;
            }
            return columnNames;
        }
        //internal static List<string> GetKeyNames(object objKey)
        //{
        //    if (objKey is DynamicParameters)
        //    {
        //        return (objKey as DynamicParameters).ParameterNames.ToList();
        //    }
        //    List<string> paramNames;
        //    if (!tableKeyMap.TryGetValue(objEntity.GetType(), out paramNames))
        //    {
        //        paramNames = new List<string>();
        //        foreach (var prop in objEntity.GetType().GetProperties(BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.Public))
        //        {
        //            var attribs = prop.GetCustomAttributes(typeof(IgnorePropertyAttribute), true);
        //            var attr = attribs.FirstOrDefault() as IgnorePropertyAttribute;
        //            if (attr == null || (attr != null && !attr.Value))
        //            {
        //                paramNames.Add(prop.Name);
        //            }
        //        }
        //        paramNameCache[objEntity.GetType()] = paramNames;
        //    }
        //    return paramNames;
        //}
    }
}
