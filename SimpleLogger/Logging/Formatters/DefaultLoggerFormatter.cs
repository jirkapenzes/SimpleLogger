namespace SimpleLogger.Logging.Formatters
{
    internal class DefaultLoggerFormatter : ILoggerFormatter
    {
        public string ApplyFormat(LogMessage logMessage)
        {
            return string.Format("{0:dd.MM.yyyy HH:mm}: {1}  [{2} -> {3}()]: {4}",
                            logMessage.DateTime, logMessage.Level, logMessage.CallingClass,
                            logMessage.CallingMethod, logMessage.Text);
        }
    }
}