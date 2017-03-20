using System;

namespace Basf.Repository.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class PrimaryKeyAttribute : Attribute
    {
        public string FieldName { get; set; }
        public Type FieldType { get; set; }
        public PrimaryKeyAttribute(string fieldName)
        {
            this.FieldName = fieldName;
        }
        public PrimaryKeyAttribute(Type fieldType)
        {
            this.FieldType = fieldType;
        }
        public PrimaryKeyAttribute(string fieldName, Type fieldType)
        {
            this.FieldName = fieldName;
            this.FieldType = fieldType;
        }
    }
}
