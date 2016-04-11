using System;

namespace Basf.Logging
{
    public interface ILoggerFactory
    {
        ILogger Create(string strName);
        ILogger Create(Type objType);
    }
}
