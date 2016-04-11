namespace Basf.Orm
{
    public class OrmProvider : IOrmProvider
    {
        public OrmProvider()
        {
        }
        public int CommandTimeout { get { return 30; } }

        public bool CreateTablesByModule { get { return false; } }

        public string ParamPrefix { get; private set; }

        public string ColumnNameFor(string propertyName)
        {
            return propertyName;
        }
        public string TableNameFor(string entityName)
        {
            return entityName;
        }
    }
}
