using System;
using System.Collections.Concurrent;
using System.Configuration;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Basf
{
    public class Utility
    {
        #region 字段&常量
        private static readonly Task<int> taskDone = Task.FromResult(1);
        private static ConcurrentDictionary<int, XmlSerializer> objSerializerList = new ConcurrentDictionary<int, XmlSerializer>();
        #endregion

        #region Config方法
        /// <summary>
        /// 获取config文件中ConnectionSettings中的数据。
        /// </summary>
        /// <param name="strConnKey">连接串Key值</param>
        /// <returns>返回Value值</returns>
        public static string GetConnString(string strConnKey)
        {
            #region GetConnString方法
            Utility.NotNull(strConnKey, "strConnKey");
            ConnectionStringSettings connSetting = ConfigurationManager.ConnectionStrings[strConnKey];
            Utility.Fail(connSetting == null || String.IsNullOrEmpty(connSetting.ConnectionString), "Config文件中,不存在{0}连接串Key！", strConnKey);
            return connSetting.ConnectionString;
            #endregion
        }
        public static string GetAppSettingValue(string strKey, string strDefault)
        {
            string strValue = ConfigurationManager.AppSettings[strKey] as string;
            if (String.IsNullOrEmpty(strValue))
            {
                return strDefault;
            }
            return strValue;
        }
        public static T GetAppSettingValue<T>(string strKey)
        {
            return Utility.GetAppSettingValue<T>(strKey, default(T));
        }
        public static T GetAppSettingValue<T>(string strKey, T objDefault)
        {
            string strValue = ConfigurationManager.AppSettings[strKey] as string;
            if (String.IsNullOrEmpty(strValue))
            {
                return objDefault;
            }
            return Utility.ConvertTo<T>(strValue);
        }
        #endregion

        #region ConvertTo
        public static T ConvertTo<T>(object objValue, T objDefault=default(T))
        {
            if (objValue == null || objValue is DBNull)
            {
                return objDefault;
            }
            Type type = typeof(T);
            if (type.IsValueType)
            {
                if (type.IsGenericType)
                {
                    type = Nullable.GetUnderlyingType(type);
                }
                if (type.IsEnum)
                {
                    if (objValue is string)
                    {
                        return (T)Enum.Parse(type, objValue as string);
                    }
                    else
                    {
                        return (T)Enum.ToObject(type, objValue);
                    }
                }
                else
                {
                    if (type == typeof(Guid))
                    {
                        object objResult = Guid.Parse(objValue.ToString());
                        return (T)objResult;
                    }
                    else if (type == typeof(TimeSpan))
                    {
                        object objResult = TimeSpan.Parse(objValue.ToString());
                        return (T)(objResult);
                    }
                    else if (typeof(IConvertible).IsAssignableFrom(type))
                    {
                        return (T)(Convert.ChangeType(objValue, type));
                    }
                }
            }
            return (T)objValue;
        }
        #endregion

        #region Guard方法
        /// <summary>
        /// 参数不允许为空，空则报ArgumentNullException异常。
        /// </summary>
        /// <param name="objArgsValue"></param>
        /// <param name="objArgsName"></param>
        public static void NotNull(object objArgsValue, string objArgsName)
        {
            if (objArgsValue == null)
            {
                throw new ArgumentNullException(objArgsName);
            }
        }
        public static Task NotNullAsync(string objArgsName)
        {
            return Task.FromException(new ArgumentNullException(objArgsName));
        }
        public static Task<TResult> NotNullAsync<TResult>(string objArgsName)
        {
            return Task.FromException<TResult>(new ArgumentNullException(objArgsName));
        }
        public static void Fail(string strFormat, params object[] objArgs)
        {
            throw new Exception(String.Format(strFormat, objArgs));
        }
        public static TResult Fail<TResult>(string strFormat, params object[] objArgs)
        {
            throw new Exception(String.Format(strFormat, objArgs));
        }
        public static Task FailAsync(string strFormat, params object[] objArgs)
        {
            return Task.FromException(new Exception(String.Format(strFormat, objArgs)));
        }
        public static Task<TResult> FailAsync<TResult>(string strFormat, params object[] objArgs)
        {
            return Task.FromException<TResult>(new Exception(String.Format(strFormat, objArgs)));
        }
        /// <summary>
        /// 当条件bCondition成立时，抛自定义异常，格式为strFormat，参数为objArgs
        /// </summary>
        /// <param name="bCondition">bool表达式</param>
        /// <param name="strFormat"></param>
        /// <param name="objArgs"></param>
        public static void Fail(bool bCondition, string strFormat, params object[] objArgs)
        {
            if (bCondition)
            {
                throw new Exception(String.Format(strFormat, objArgs));
            }
        }
        #endregion

        #region TaskDone
        public static Task TaskDone
        {
            get
            {
                return taskDone;
            }
        }
        #endregion
    }
}
