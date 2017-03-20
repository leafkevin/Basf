using System.Data.Common;

namespace Basf.Repository
{
    public interface IOrmProvider
    {
        string ParamPrefix { get; }
        DbConnection CreateConnection(string ConnString);
        string GetQuotedTableName(string tableName);
        string GetQuotedColumnName(string columnName);
        string GetPagingExpression(int skip, int? limit, string orderBy = null);
    }
}
