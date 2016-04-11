namespace Basf.Orm
{
    public interface IOrmProvider
    {
        string ParamPrefix { get; }
        int CommandTimeout { get; }
        bool CreateTablesByModule { get; }
        string TableNameFor(string entityName);
        string ColumnNameFor(string propertyName);
    }
}
