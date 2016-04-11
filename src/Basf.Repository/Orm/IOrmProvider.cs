namespace Basf.Repository.Orm
{
    public interface IOrmProvider
    {
        string ParamString { get; set; }
        int CommandTimeout { get; set; }
        bool CreateTablesByModule { get; set; }
        string TableNameFor(string entityName);
        string ColumnNameFor(string propertyName);
    }
}
