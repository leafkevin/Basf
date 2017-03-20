using System;

namespace Basf.Orm
{
    [AttributeUsage(AttributeTargets.Property)]
    public class TableAttribute : Attribute
    {
        public string TableName { get; set; }
        public string ColumnPrefix { get; set; }
        public string ColumnSuffix { get; set; }
        public TableAttribute(string tableName, string columnPrefix = null, string columnSuffix = null)
        {
            this.TableName = tableName;
            this.ColumnPrefix = columnPrefix;
            this.ColumnSuffix = columnSuffix;
        }
    }
}