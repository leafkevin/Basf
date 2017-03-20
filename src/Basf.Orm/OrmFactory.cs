using System.Data.Common;

namespace Basf.Orm
{
    public class OrmFactory
    {
        public string ConnString { get; set; }
        public DbProviderFactory Factory { get; set; }
        public OrmFactory(string connString, DbProviderFactory factory)
        {
            this.ConnString = connString;
            this.Factory = factory;
        }
        public DbConnection CreateConnection()
        {
            var connection = this.Factory.CreateConnection();
            connection.ConnectionString = this.ConnString;
            return connection;
        }
    }
}
