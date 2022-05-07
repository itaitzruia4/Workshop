using System;
using System.IO;
using System.Reflection;

namespace Workshop.DomainLayer.Loggers
{
    public sealed class Logger
    {
        private static log4net.ILog log;
        private Logger()
        {
            string fileName = "logs.log";
            log4net.GlobalContext.Properties["LogName"] = fileName; //Log file path
            log4net.Config.XmlConfigurator.Configure();
            log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        }

        private static readonly Lazy<Logger> instance = new Lazy<Logger>(() => new Logger());

        public static Logger Instance
        {
            get{
                return instance.Value;
            }
        }

        public void LogEvent(string message)
        {
            log.Info(message);
        }

        public void LogError(string message)
        {
            log.Error(message);
        }

        public void LogDebug(string message)
        {
            log.Debug(message);
        }
    }
}
