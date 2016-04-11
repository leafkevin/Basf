using System;

namespace Basf.Logging
{
    public class EmptyLogger : ILogger
    {
        #region ILogger Members
        public bool IsInfoEnabled { get { return false; } }
        public bool IsDebugEnabled { get { return false; } }
        public bool IsErrorEnabled { get { return false; } }
        public void Debug(object objMessage)
        {
        }
        public void DebugFormat(string strFormat, params object[] objArgs)
        {
        }
        public void Debug(object objMessage, Exception objException)
        {
        }
        public void Info(object objMessage)
        {
        }
        public void InfoFormat(string strFormat, params object[] objArgs)
        {
        }
        public void Info(object objMessage, Exception objException)
        {
        }
        public void Error(object objMessage)
        {
        }
        public void ErrorFormat(string strFormat, params object[] objArgs)
        {
        }
        public void Error(object objMessage, Exception objException)
        {
        }
        public void Warn(object objMessage)
        {
        }
        public void WarnFormat(string strFormat, params object[] objArgs)
        {
        }
        public void Warn(object objMessage, Exception objException)
        {
        }
        public void Fatal(object objMessage)
        {
        }
        public void FatalFormat(string strFormat, params object[] objArgs)
        {
        }
        public void Fatal(object objMessage, Exception objException)
        {
        }
        #endregion
    }
}