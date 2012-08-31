namespace SimpleLogger.Logging
{
    public interface ILoggerHandlerManager
    {
        ILoggerHandlerManager AddHandler(ILoggerHandler loggerHandler);

        bool RemoveHandler(ILoggerHandler loggerHandler);
    }
}
