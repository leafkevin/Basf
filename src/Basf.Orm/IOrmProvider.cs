using System.Data.Common;

namespace Basf.Orm
{
    public interface IOrmProvider
    {
        int CommandTimeout { get; }
        bool CreateTablesByModule { get; }
        string TableNamePrefix { get; }
        string TableNameSuffix { get; }
        string ColumnNamePrefix { get; }
        string ColumnNameSuffix { get; }
        DbConnection CreateConnection(string connKey);
    }
}
