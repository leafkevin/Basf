namespace Basf.Logging
{
    public interface ILogger
    {
        void Debug(string message);
        void Debug(string format, params object[] objArgs);
        void Info(string message);
        void Info(string strFormat, params object[] objArgs);
        void Error(string message);
        void Error(string strFormat, params object[] objArgs);
        void Warn(string message);
        void Warn(string strFormat, params object[] objArgs);
        void Fatal(string message);
        void Fatal(string strFormat, params object[] objArgs);
    }
}