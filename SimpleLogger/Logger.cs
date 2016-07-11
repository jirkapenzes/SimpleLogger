using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using SimpleLogger.Logging;
using SimpleLogger.Logging.Handlers;
using SimpleLogger.Logging.Module;

namespace SimpleLogger
{
    public static class Logger
    {
        private static readonly LogPublisher LogPublisher;
        private static readonly ModuleManager ModuleManager;
        private static readonly DebugLogger DebugLogger;

        private static readonly object Sync = new object();
        private static Level _defaultLevel = Level.Info;
        private static bool _isTurned = true;
        private static bool _isTurnedDebug = true;

        public enum Level
        {
            None,
            Debug,
            Fine,
            Info,
            Warning,
            Error,
            Severe
        }

        static Logger()
        {
            lock (Sync)
            {
                LogPublisher = new LogPublisher();
                ModuleManager = new ModuleManager();
                DebugLogger = new DebugLogger();
            }
        }

        public static void DefaultInitialization()
        {
            LoggerHandlerManager
                .AddHandler(new ConsoleLoggerHandler())
                .AddHandler(new FileLoggerHandler());

            Log(Level.Info, "Default initialization");
        }

        public static Level DefaultLevel
        {
            get { return _defaultLevel; }
            set { _defaultLevel = value; }
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
            var stackFrame = FindStackFrame();
            var methodBase = GetCallingMethodBase(stackFrame);
            var callingMethod = methodBase.Name;
            var callingClass = methodBase.ReflectedType.Name;
            var lineNumber = stackFrame.GetFileLineNumber();

            Log(level, message, callingClass, callingMethod, lineNumber);
        }

        public static void Log(Exception exception)
        {
            Log(Level.Error, exception.Message);
            ModuleManager.ExceptionLog(exception);
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
            var stackFrame = FindStackFrame();
            var methodBase = GetCallingMethodBase(stackFrame);
            var callingMethod = methodBase.Name;
            var callingClass = typeof(TClass).Name;
            var lineNumber = stackFrame.GetFileLineNumber();

            Log(level, message, callingClass, callingMethod, lineNumber);
        }

        private static void Log(Level level, string message, string callingClass, string callingMethod, int lineNumber)
        {
            if (!_isTurned || (!_isTurnedDebug && level == Level.Debug))
                return;

            var currentDateTime = DateTime.Now;

            ModuleManager.BeforeLog();
            var logMessage = new LogMessage(level, message, currentDateTime, callingClass, callingMethod, lineNumber);
            LogPublisher.Publish(logMessage);
            ModuleManager.AfterLog(logMessage);
        }

        private static MethodBase GetCallingMethodBase(StackFrame stackFrame)
        {
            return stackFrame == null
                ? MethodBase.GetCurrentMethod() : stackFrame.GetMethod();
        }

        private static StackFrame FindStackFrame()
        {
            var stackTrace = new StackTrace();
            for (var i = 0; i < stackTrace.GetFrames().Count(); i++)
            {
                var methodBase = stackTrace.GetFrame(i).GetMethod();
                var name = MethodBase.GetCurrentMethod().Name;
                if (!methodBase.Name.Equals("Log") && !methodBase.Name.Equals(name))
                    return new StackFrame(i, true);
            }
            return null;
        }

        public static void On()
        {
            _isTurned = true;
        }

        public static void Off()
        {
            _isTurned = false;
        }

        public static void DebugOn()
        {
            _isTurnedDebug = true;
        }

        public static void DebugOff()
        {
            _isTurnedDebug = false;
        }

        public static IEnumerable<LogMessage> Messages
        {
            get { return LogPublisher.Messages; }
        }

        public static DebugLogger Debug
        {
            get { return DebugLogger; }
        }

        public static ModuleManager Modules
        {
            get { return ModuleManager; }
        }

        public static bool StoreLogMessages
        { 
            get { return LogPublisher.StoreLogMessages; }
            set { LogPublisher.StoreLogMessages = value; }
        }

        static class FilterPredicates
        {
            public static bool ByLevelHigher(Level logMessLevel, Level filterLevel)
            {
                return ((int)logMessLevel >= (int)filterLevel);
            }

            public static bool ByLevelLower(Level logMessLevel, Level filterLevel)
            {
                return ((int)logMessLevel <= (int)filterLevel);
            }

            public static bool ByLevelExactly(Level logMessLevel, Level filterLevel)
            {
                return ((int)logMessLevel == (int)filterLevel);
            }

            public static bool ByLevel(LogMessage logMessage, Level filterLevel, Func<Level, Level, bool> filterPred)
            {
                return filterPred(logMessage.Level, filterLevel);
            }
        }

        public class FilterByLevel
        {
            public Level FilteredLevel { get; set; }
            public bool ExactlyLevel { get; set; }
            public bool OnlyHigherLevel { get; set; }

            public FilterByLevel(Level level)
            {
                FilteredLevel = level;
                ExactlyLevel = true;
                OnlyHigherLevel = true;
            }

            public FilterByLevel() 
            {
                ExactlyLevel = false;
                OnlyHigherLevel = true;
            }

            public Predicate<LogMessage> Filter { get { return delegate(LogMessage logMessage) {
                return FilterPredicates.ByLevel(logMessage, FilteredLevel, delegate(Level lm, Level fl) {
                return ExactlyLevel ? 
                    FilterPredicates.ByLevelExactly(lm, fl) : 
                    (OnlyHigherLevel ? 
                        FilterPredicates.ByLevelHigher(lm, fl) : 
                        FilterPredicates.ByLevelLower(lm, fl)
                    );
                    });
                }; 
            }  }
        }
    }
}
