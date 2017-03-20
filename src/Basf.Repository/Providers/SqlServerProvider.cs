using System.Data.Common;

namespace Basf.Repository.Providers
{
    public class SqlServerProvider : IOrmProvider
    {
        public SqlServerProvider()
        {
        }
        public string ParamPrefix { get { return "@"; } }
        public DbConnection CreateConnection(string connString)
        {
            var factory = OrmProviderFactory.GetFactory("System.Data.SqlClient.SqlClientFactory, System.Data, Version = 4.0.0.0, Culture = neutral, PublicKeyToken = b77a5c561934e089");
            var result = factory.CreateConnection();
            result.ConnectionString = connString;
            return result;
        }
        public string GetQuotedColumnName(string columnName)
        {
            return "[" + columnName + "]";
        }
        public string GetQuotedTableName(string tableName)
        {
            return "[" + tableName + "]";
        }
        public string GetPagingExpression(int skip, int? limit, string orderBy = null)
        {
            return limit.HasValue ? string.Format(" {0} OFFSET {1} ROWS FETCH NEXT {2} ROWS ONLY", orderBy ?? "", skip, limit) :
              string.Format(" {0} OFFSET {1} ROWS", orderBy ?? "", skip);
        }
    }
}
