using Basf.Logging;
using log4net;
using log4net.Config;
using System;
using System.IO;

namespace Basf.Log4net
{
    public class Log4NetLoggerFactory : ILoggerFactory
    {
        public Log4NetLoggerFactory(string configFile = null)
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
        }
        public ILogger Create(string name)
        {
            return new Log4NetLogger(LogManager.GetLogger(name));
        }
        public ILogger Create(Type type)
        {
            return new Log4NetLogger(LogManager.GetLogger(type));
        }
    }
}
