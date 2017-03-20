using System.Data.Common;

namespace Basf.Repository.Providers
{
    public class OracleProvider : IOrmProvider
    {
        public OracleProvider()
        {
        }
        public string ParamPrefix { get { return ":"; } }
        public DbConnection CreateConnection(string connString)
        {
            var factory = OrmProviderFactory.GetFactory("Oracle.ManagedDataAccess.Client.OracleClientFactory, Oracle.ManagedDataAccess, Culture=neutral, PublicKeyToken=89b483f429c47342");
            var result = factory.CreateConnection();
            result.ConnectionString = connString;
            return result;
        }
        public string GetQuotedColumnName(string columnName)
        {
            return "\"" + columnName + "\"";
        }
        public string GetQuotedTableName(string tableName)
        {
            return "\"" + tableName + "\"";
        }
        public string GetPagingExpression(int skip, int? limit, string orderBy = null)
        {
            return "";
        }
    }
}
