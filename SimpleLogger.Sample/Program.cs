using System;
using SimpleLogger.Logging.Handlers;
using SimpleLogger.Logging.Module;
using SimpleLogger.Logging.Module.Database;

namespace SimpleLogger.Sample
{
    class Program
    {
        public static void Main()
        {
            // Adding handler - to show log messages (ILoggerHandler)
            Logger.LoggerHandlerManager
                .AddHandler(new ConsoleLoggerHandler())
                .AddHandler(new FileLoggerHandler())
                .AddHandler(new DebugConsoleLoggerHandler());

            // Fast logging (monitor name of class and methods from which is the application logged)
            Logger.Log();

            // We define a log message
            Logger.Log("Hello world");

            // We can define the level (type) of message
            Logger.Log(Logger.Level.Fine, "Explicit define level");

            // Explicit definition of the class from which the logging
            Logger.Log<Program>("Explicit define log class");
            Logger.Log<Program>(Logger.Level.Fine, "Explicit define log class and level");

            // Settings of default type of message
            Logger.DefaultLevel = Logger.Level.Severe;

            try
            {
                // Simulation of exceptions
                throw new Exception();
            }
            catch (Exception exception)
            {
                // Logging exceptions
                // Automatical adjustment of specific log into the Error and adding of StackTrace
                Logger.Log(exception);
                Logger.Log<Program>(exception);
            }

            // Special feature - debug logging

            Logger.Debug.Log("Debug log");
            Logger.Debug.Log<Program>("Debug log");

            Logger.DebugOff();
            Logger.Debug.Log("Not-logged message");

            Logger.DebugOn();
            Logger.Debug.Log("I'am back!");

            Console.ReadKey();
        }

        private static void MySqlDatabaseLoggerModuleSample()
        {
            // Just add the module and it works! 
            Logger.Modules.Install(new DatabaseLoggerModule(DatabaseType.MySql, "Your connection string here!"));
            Logger.Log("My first database log! ");
        }

        private static void MsSqlDatabaseLoggerModuleSample()
        {
            // Just add the module and it works! 
            Logger.Modules.Install(new DatabaseLoggerModule(DatabaseType.MsSql, "Your connection string here!"));
            Logger.Log("My first database log! ");
        }

        private static void OracleDatabaseLoggerModuleSample()
        {
            // Just add the module and it works! 
            Logger.Modules.Install(new DatabaseLoggerModule(DatabaseType.Oracle, "Your connection string here!"));
            Logger.Log("My first database log! ");
        }

        public static void EmailModuleSample()
        {
            // Configuring smtp server
            var smtpServerConfiguration = new SmtpServerConfiguration("userName", "password", "smtp.gmail.com", 587);

            // Creating a module
            var emailSenderLoggerModule = new EmailSenderLoggerModule(smtpServerConfiguration)
            {
                EnableSsl = true,
                Sender = "sender-email@gmail.com"
            };

            // Add the module and it works
            emailSenderLoggerModule.Recipients.Add("recipients@gmail.com");

            //  // Simulation of exceptions
            Logger.Modules.Install(emailSenderLoggerModule);

            try
            {
                // Simulation of exceptions
                throw new NullReferenceException();
            }
            catch (Exception exception)
            {
                // Log exception
                // If you catch an exception error -> will be sent an email with a list of log message.
                Logger.Log(exception);
            }
        }
    }
}
