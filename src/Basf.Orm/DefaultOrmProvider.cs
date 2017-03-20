using System.Data.Common;
using System.Configuration;
using System.Collections.Generic;

namespace Basf.Orm
{
    public class DefaultOrmProvider : IOrmProvider
    {
        private Dictionary<string, OrmFactory> factorys = new Dictionary<string, OrmFactory>();
        public DefaultOrmProvider()
        {
        }
        public int CommandTimeout { get { return 30; } }
        public bool CreateTablesByModule { get { return false; } }
        public string ParamPrefix { get; private set; } = "@";
        public string ColumnNameFor(string propertyName)
        {
            return "[" + propertyName + "]";
        }
        public DbConnection CreateConnection(string connKey)
        {
            if (!this.factorys.ContainsKey(connKey))
            {
                ConnectionStringSettings connSetting = ConfigurationManager.ConnectionStrings[connKey];
                if (connSetting != null)
                {
                    var factory = DbProviderFactories.GetFactory(connSetting.ProviderName);
                    this.factorys.Add(connKey, new OrmFactory(connSetting.ConnectionString, factory));
                }
            }
            return this.factorys[connKey].CreateConnection();
        }
        public string TableNameFor(string entityName)
        {
            return "[" + entityName + "]";
        }
    }
}
