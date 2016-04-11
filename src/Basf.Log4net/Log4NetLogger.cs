using Basf.Logging;
using log4net;
using log4net.Config;
using System;
using System.IO;

namespace Basf.Log4net
{
    public class Log4NetLogger : ILogger
    {
        private readonly ILog objInfoLogger;
        private readonly ILog objDebugLogger;
        private readonly ILog objErrorLogger;
        public Log4NetLogger(string configFile = null)
        {
            if (!String.IsNullOrEmpty(configFile))
            {
                if (!File.Exists(configFile))
                {
                    configFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, configFile);
                }
                if (File.Exists(configFile))
                {
                    XmlConfigurator.ConfigureAndWatch(new FileInfo(configFile));
                }
            }
            else
            {
                XmlConfigurator.Configure();
            }
            this.objInfoLogger = LogManager.GetLogger("InfoLogger");
            this.objDebugLogger = LogManager.GetLogger("DebugLogger");
            this.objErrorLogger = LogManager.GetLogger("ErrorLogger");
        }
        #region ILogger Members
        public bool IsInfoEnabled { get { return this.objInfoLogger.IsInfoEnabled; } }
        public bool IsDebugEnabled { get { return this.objDebugLogger.IsDebugEnabled; } }
        public bool IsErrorEnabled { get { return this.objErrorLogger.IsErrorEnabled; } }

        public void Info(object objMessage)
        {
            this.objInfoLogger.Info(objMessage);
        }
        public void InfoFormat(string strFormat, params object[] objArgs)
        {
            this.objInfoLogger.InfoFormat(strFormat, objArgs);
        }
        public void Info(object objMessage, Exception objException)
        {
            this.objInfoLogger.Info(objMessage, objException);
        }
        public void Warn(object objMessage)
        {
            this.objDebugLogger.Warn(objMessage);
        }
        public void WarnFormat(string strFormat, params object[] objArgs)
        {
            this.objDebugLogger.WarnFormat(strFormat, objArgs);
        }
        public void Warn(object objMessage, Exception objException)
        {
            this.objDebugLogger.Warn(objMessage, objException);
        }
        public void Debug(object objMessage)
        {
            this.objDebugLogger.Debug(objMessage);
        }
        public void DebugFormat(string strFormat, params object[] objArgs)
        {
            this.objDebugLogger.DebugFormat(strFormat, objArgs);
        }
        public void Debug(object objMessage, Exception objException)
        {
            this.objDebugLogger.Debug(objMessage, objException);
        }
        public void Error(object objMessage)
        {
            this.objErrorLogger.Error(objMessage);
        }
        public void ErrorFormat(string strFormat, params object[] objArgs)
        {
            this.objErrorLogger.ErrorFormat(strFormat, objArgs);
        }
        public void Error(object objMessage, Exception objException)
        {
            this.objErrorLogger.Error(objMessage, objException);
        }
        public void Fatal(object objMessage)
        {
            this.objErrorLogger.Fatal(objMessage);
        }
        public void FatalFormat(string strFormat, params object[] objArgs)
        {
            this.objErrorLogger.FatalFormat(strFormat, objArgs);
        }
        public void Fatal(object objMessage, Exception objException)
        {
            this.objErrorLogger.Fatal(objMessage, objException);
        }
        #endregion
    }
}
