using System.Data.Common;

namespace Basf.Repository.Providers
{
    public class PostgreSqlProvider : IOrmProvider
    {
        public PostgreSqlProvider()
        {
        }
        public string ParamPrefix { get { return "@"; } }
        public DbConnection CreateConnection(string connString)
        {
            var factory = OrmProviderFactory.GetFactory("Npgsql.NpgsqlFactory, Npgsql, Culture=neutral, PublicKeyToken=5d8b90d52f46fda7");
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
            return limit.HasValue ? string.Format(" {0} LIMIT {1} OFFSET {2}", orderBy ?? "", limit, skip) :
               string.Format(" {0} OFFSET {1}", orderBy ?? "", skip);
        }
    }
}

