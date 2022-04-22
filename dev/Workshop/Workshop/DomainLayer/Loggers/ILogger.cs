namespace Workshop.DomainLayer.Loggers
{
    interface ILogger{
        void LogEvent(string message);
        void LogError(string message);
        void LogDebug(string message);
    }
}