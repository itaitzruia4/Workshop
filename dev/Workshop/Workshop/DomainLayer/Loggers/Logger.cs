using System;
using System.IO;
using System.Reflection;

namespace Workshop.DomainLayer.Loggers
{
    public class Logger: Workshop.DomainLayer.Loggers.ILogger
    {
        private static readonly log4net.ILog log;
        static Logger()
        {
            if (log == null)
            {
                string fileName = "logs";
                string basePath = new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath.Split(new string[] { "\\Logs" }, StringSplitOptions.None)[0];
                string filePath = Path.Combine(basePath, $"Logs\\{fileName}.log");
                log4net.GlobalContext.Properties["LogFileName"] = filePath; //Log file path
                log4net.Config.XmlConfigurator.Configure();
                log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
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

        public void LogDebug(string mesesage)
        {
            log.Debug(mesesage);
        }
    }
}
