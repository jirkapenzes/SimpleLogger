Simple Logger C#.NET
=============

Very easy to use.

Usage
-------
```csharp
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
```

#### Output
```
12.10.2012 21:43: Info [line: 18 Program -> Main()]: There is no message
12.10.2012 21:43: Info [line: 21 Program -> Main()]: Hello world
12.10.2012 21:43: Fine [line: 24 Program -> Main()]: Explicit define level
12.10.2012 21:43: Info [line: 27 Program -> Main()]: Explicit define log class
12.10.2012 21:43: Fine [line: 28 Program -> Main()]: Explicit define log class and level
12.10.2012 21:43: Error [line: 35 Program -> Main()]: Exception of type 'System.Exception' was thrown.
12.10.2012 21:43: Error [line: 35 Program -> Main()]: Log exception -> Message: Exception of type 'System.Exception' was thrown.
StackTrace:    at SimpleLogger.Sample.Program.Main() in c:\GitHub\SimpleLogger\SimpleLogger.Sample\Program.cs:line 35

12.10.2012 21:43: Debug [line: 47 Program -> Main()]: Debug log
12.10.2012 21:43: Debug [line: 48 Program -> Main()]: Debug log
12.10.2012 21:43: Debug [line: 55 Program -> Main()]: I'am back!

```


Modules
-------
### Email module

```csharp
    // Email module sample
    public static void EmaiLModuleSample()
    {
        // Configuring smtp server
        var smtpServerConfiguration = new SmtpServerConfiguration("userName", "password", "smtp.gmail.com", 587);

        // Creating a module
        var emailSenderLoggerModule = new EmailSenderLoggerModule(smtpServerConfiguration)
        {
            EnableSsl = true,
            Sender = "sender-email@gmail.com"
        };

        // Adding recipients
        emailSenderLoggerModule.Recipients.Add("recipients@gmail.com");

        // Add the module and it works
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
```

### MS SQL database module
```csharp
    // MS SQL database module sample
    public static void MsSqlDatabaseLoggerModuleSample()
    {
        var connectionString = "Your connection string";

        // Just add the module and it works! 
        Logger.Modules.Install(new MsSqlDatabaseLoggerModule(connectionString));
        Logger.Log("My first database log! ");
    }
```