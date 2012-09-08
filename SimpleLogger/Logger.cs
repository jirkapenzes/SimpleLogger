using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using SimpleLogger.Logging;
using SimpleLogger.Logging.Module;

namespace SimpleLogger
{
    public static class Logger
    {
        private static readonly LogPublisher LogPublisher;
        private static readonly ModuleManager ModuleManager;

        private static readonly object Sync = new object();
        private static Level _defaultLevel = Level.Info;
        private static bool _isTurned = true;

        public enum Level
        {
            None,
            Info,
            Warning,
            Error,
            Severe,
            Fine,
            Debug
        }

        static Logger()
        {
            lock (Sync)
            {
                LogPublisher = new LogPublisher();
                ModuleManager = new ModuleManager();
            }
        }

        public static void SetDefaultLevel(Level level = Level.Info)
        {
            _defaultLevel = level;
        }

        public static ILoggerHandlerManager LoggerHandlerManager
        {
            get { return LogPublisher; }
        }

        public static void Log()
        {
            Log("There is no message");
        }

        public static void Log(string message)
        {
            Log(_defaultLevel, message);
        }

        public static void Log(Level level, string message)
        {
            var methodBase = GetCallingMethodBase();
            var callingMethod = methodBase.Name;
            var callingClass = methodBase.ReflectedType.Name;

            Log(level, message, callingClass, callingMethod);
        }

        public static void Log(Exception exception)
        {
            ModuleManager.ExceptionLog(exception);
            Log(Level.Error, exception.Message);
        }

        public static void Log<TClass>(Exception exception) where TClass : class
        {
            var message = string.Format("Log exception -> Message: {0}\nStackTrace: {1}", exception.Message,
                                        exception.StackTrace);
            Log<TClass>(Level.Error, message);
        }

        public static void Log<TClass>(string message) where TClass : class
        {
            Log<TClass>(_defaultLevel, message);
        }

        public static void Log<TClass>(Level level, string message) where TClass : class
        {
            var methodBase = GetCallingMethodBase();
            var callingMethod = methodBase.Name;
            var callingClass = typeof(TClass).Name;

            Log(level, message, callingClass, callingMethod);
        }

        private static void Log(Level level, string message, string callingClass, string callingMethod)
        {
            if (!_isTurned)
                return;

            var currentDateTime = DateTime.Now;

            ModuleManager.BeforeLog();
            var logMessage = new LogMessage(level, message, currentDateTime, callingClass, callingMethod);

            LogPublisher.Publish(logMessage);
            ModuleManager.AfterLog(logMessage);
        }

        private static MethodBase GetCallingMethodBase()
        {
            var stackTrace = new StackTrace();
            for (var i = 0; i < stackTrace.GetFrames().Count(); i++)
            {
                var methodBase = stackTrace.GetFrame(i).GetMethod();
                var name = MethodBase.GetCurrentMethod().Name;
                if (!methodBase.Name.Equals("Log") && !methodBase.Name.Equals(name))
                    return methodBase;
            }
            return MethodBase.GetCurrentMethod();
        }

        public static void On()
        {
            _isTurned = true;
        }

        public static void Off()
        {
            _isTurned = false;
        }

        public static IList<LoggerModule> Modules
        {
            get { return ModuleManager.Modules; }
        }

        public static IEnumerable<LogMessage> Messages
        {
            get { return LogPublisher.Messages; }
        }
    }
}
