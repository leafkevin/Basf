using System;

namespace Basf.Logging
{
    public class EmptyLoggerFactory : ILoggerFactory
    {
        private static readonly EmptyLogger Logger = new EmptyLogger();
        public ILogger Create(string strNme)
        {
            return Logger;
        }
        public ILogger Create(Type objType)
        {
            return Logger;
        }
    }
}
