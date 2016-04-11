using System;

namespace Basf.Repository.Orm
{
    [AttributeUsage(AttributeTargets.Property)]
    public class TableAttribute : Attribute
    {
        public string TableName { get; set; }

        public TableAttribute(string tableName)
        {
            this.TableName = tableName;
        }
    }
}