using System;

namespace Basf.Repository.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ColumnAttribute : Attribute
    {
        public string FieldName { get; set; }
        public Type FieldType { get; set; }
        public ColumnAttribute(string fieldName)
        {
            this.FieldName = fieldName;
        }
        public ColumnAttribute(Type fieldType)
        {
            this.FieldType = fieldType;
        }
        public ColumnAttribute(string fieldName, Type fieldType)
        {
            this.FieldName = fieldName;
            this.FieldType = fieldType;
        }
    }
}
