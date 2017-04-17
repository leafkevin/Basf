using System;

namespace Basf
{
    public class AopMatcher
    {
        public AopMatchKind Kind { get; set; }
        public string Expression { get; set; }
        public Type InterceptorType { get; set; }
    }
    public enum AopMatchKind
    {
        ClassName = 1,
        InterfaceName = 2,
        AssignableFrom = 3,
        MethodName = 4
    }
}
