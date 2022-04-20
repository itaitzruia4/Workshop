namespace Workshop.DomainLayer.Loggers
{
    public class Logger
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public Logger()
        {

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
