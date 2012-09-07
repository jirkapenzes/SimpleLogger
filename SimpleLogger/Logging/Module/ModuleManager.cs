using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleLogger.Logging.Module
{
    public class ModuleManager : IDisposable
    {
        private readonly IList<LoggerModule> _modules;

        public ModuleManager()
        {
            _modules = new List<LoggerModule>();
        }

        public void BeforeLog()
        {
            foreach (var loggerModule in _modules)
                loggerModule.BeforeLog();
        }

        public void AfterLog(LogMessage logMessage)
        {
            foreach (var loggerModule in _modules)
                loggerModule.AfterLog(logMessage);
        }

        public void ExceptionLog(Exception exception)
        {
            foreach (var loggerModule in _modules)
                loggerModule.ExceptionLog(exception);
        }

        public void Dispose()
        {
            foreach (var loggerModule in Modules)
                loggerModule.Dispose();
        }

        public IList<LoggerModule> Modules
        {
            get { return _modules; }
        }


    }
}
