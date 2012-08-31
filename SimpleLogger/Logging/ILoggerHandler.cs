namespace SimpleLogger.Logging
{
    public interface ILoggerHandler
    {
        void Publish(LogMessage logMessage);
    }
}