using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Text;
using SimpleLogger.Logging.Formatters;

namespace SimpleLogger.Logging.Module
{
    public class EmailSenderLoggerModule : LoggerModule
    {
        private readonly SmtpServerConfiguration _smtpServerConfiguration;

        public string Sender { get; set; }
        public IList<string> Recipients { get; private set; }
        public bool EnableSsl { get; set; }

        private readonly string _subject;
        private readonly ILoggerFormatter _loggerFormatter;

        public EmailSenderLoggerModule(SmtpServerConfiguration smtpServerConfiguration)
            : this(smtpServerConfiguration, GenerateSubjectName()) { }

        public EmailSenderLoggerModule(SmtpServerConfiguration smtpServerConfiguration, string subject)
            : this(smtpServerConfiguration, subject, new DefaultLoggerFormatter()) { }

        public EmailSenderLoggerModule(SmtpServerConfiguration smtpServerConfiguration, ILoggerFormatter loggerFormatter)
            : this(smtpServerConfiguration, GenerateSubjectName(), loggerFormatter) { }

        public EmailSenderLoggerModule(SmtpServerConfiguration smtpServerConfiguration, string subject, ILoggerFormatter loggerFormatter)
        {
            _smtpServerConfiguration = smtpServerConfiguration;
            _subject = subject;
            _loggerFormatter = loggerFormatter;

            Recipients = new List<string>();
        }

        public override string Name
        {
            get { return "EmailSenderLoggerModule"; }
        }

        public override void ExceptionLog(Exception exception)
        {
            if (string.IsNullOrEmpty(Sender) || Recipients.Count == 0)
                throw new NullReferenceException("Not specified email sender and recipient. ");

            var body = MakeEmailBodyFromLogHistory();
            var client = new SmtpClient(_smtpServerConfiguration.Host, _smtpServerConfiguration.Port)
            {
                EnableSsl = EnableSsl,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(_smtpServerConfiguration.UserName, _smtpServerConfiguration.Password)
            };

            foreach (var recipient in Recipients)
            {
                using (var mailMessage = new MailMessage(Sender, recipient, _subject, body))
                {
                    client.Send(mailMessage);
                }
            }
        }

        private static string GenerateSubjectName()
        {
            var currentDate = DateTime.Now;
            return string.Format("SimpleLogger {0} {1}", currentDate.ToShortDateString(), currentDate.ToShortTimeString());
        }

        private string MakeEmailBodyFromLogHistory()
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("Simple logger - email module");

            foreach (var logMessage in Logger.Messages)
                stringBuilder.AppendLine(_loggerFormatter.ApplyFormat(logMessage));

            return stringBuilder.ToString();
        }
    }

    public class SmtpServerConfiguration
    {
        public string UserName { get; private set; }
        public string Password { get; private set; }
        public string Host { get; private set; }
        public int Port { get; private set; }

        public SmtpServerConfiguration(string userName, string password, string host, int port)
        {
            UserName = userName;
            Password = password;
            Host = host;
            Port = port;
        }
    }
}
