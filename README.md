Simple Logger C#.NET
=============

Very easy to use.

Usage
-------

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
        Logger.Log(Logger.Level.Fine, "Hello world");

        // Explicit definition of the class from which the logging
        Logger.Log<Program>("Hello world");
        Logger.Log<Program>(Logger.Level.Fine, "Hello world");

        // Settings of default type of message
        Logger.SetDefaultLevel(Logger.Level.Severe);

        try { }
        catch (Exception exception)
        {
            // Logging exceptions
            // Automatical adjustment of specific log into the Error and adding of StackTrace
            Logger.Log(exception);
            Logger.Log<Program>(exception);
        }
    }
