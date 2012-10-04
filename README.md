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
        Logger.Log(Logger.Level.Fine, "Hello world");

        // Explicit definition of the class from which the logging
        Logger.Log<Program>("Hello world");
        Logger.Log<Program>(Logger.Level.Fine, "Hello world");

        // Settings of default type of message
        Logger.DefaultLevel = Logger.Level.Severe;

        try { }
        catch (Exception exception)
        {
            // Logging exceptions
            // Automatical adjustment of specific log into the Error and adding of StackTrace
            Logger.Log(exception);
            Logger.Log<Program>(exception);
        }

    }


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
        Logger.Modules.Add(emailSenderLoggerModule);

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
        Logger.Modules.Add(new MsSqlDatabaseLoggerModule(connectionString));
        Logger.Log("My first database log! ");
    }
```