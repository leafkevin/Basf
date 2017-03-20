using System.Data.Common;

namespace Basf.Repository.Providers
{
    public class SqlLiteProvider : IOrmProvider
    {
        public SqlLiteProvider()
        {
        }
        public string ParamPrefix { get { return "@"; } }
        public DbConnection CreateConnection(string connString)
        {
            var factory = OrmProviderFactory.GetFactory("System.Data.SQLite.SQLiteFactory, System.Data.SQLite, Culture=neutral, PublicKeyToken=db937bc2d44ff139");
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

