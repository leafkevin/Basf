using System;

namespace Basf.Orm
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ComputeAttribute : Attribute
    {
        public string Expression { get; set; }
        public ComputeAttribute() : this(string.Empty)
        {
        }
        public ComputeAttribute(string expression)
        {
            Expression = expression;
        }
    }
}
