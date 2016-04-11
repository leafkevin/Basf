using System;

namespace Basf.Logging
{
    public interface ILogger
    {
        bool IsInfoEnabled { get; }
        bool IsDebugEnabled { get; }
        bool IsErrorEnabled { get; }
        void Debug(object objMessage);
        void DebugFormat(string strFormat, params object[] objArgs);
        void Debug(object objMessage, Exception objException);
        void Info(object objMessage);
        void InfoFormat(string strFormat, params object[] objArgs);
        void Info(object objMessage, Exception objException);
        void Error(object objMessage);
        void ErrorFormat(string strFormat, params object[] objArgs);
        void Error(object objMessage, Exception objException);
        void Warn(object objMessage);
        void WarnFormat(string strFormat, params object[] objArgs);
        void Warn(object objMessage, Exception objException);
        void Fatal(object objMessage);
        void FatalFormat(string strFormat, params object[] objArgs);
        void Fatal(object objMessage, Exception objException);
    }
}