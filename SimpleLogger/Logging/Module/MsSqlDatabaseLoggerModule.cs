using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace SimpleLogger.Logging.Module
{
    public class MsSqlDatabaseLoggerModule : LoggerModule
    {
        private readonly string _connectionString;
        private readonly string _tableName;

        public MsSqlDatabaseLoggerModule(string connectionString) : this(connectionString, "Log") { }

        public MsSqlDatabaseLoggerModule(string connectionString, string tableName)
        {
            _connectionString = connectionString;
            _tableName = tableName;
        }

        public override void Initialize()
        {
            CreateTable();
        }

        public override string Name
        {
            get { return "MsSqlDatabaseLoggerModule"; }
        }

        public override void AfterLog(LogMessage logMessage)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var commandText = MakeInsertSqlCommand();
                var sqlCommand = new SqlCommand(commandText, connection);
                sqlCommand.Parameters.AddWithValue("@text", logMessage.Text);
                sqlCommand.Parameters.AddWithValue("@dateTime", logMessage.DateTime);
                sqlCommand.Parameters.AddWithValue("@level", logMessage.Level.ToString());
                sqlCommand.Parameters.AddWithValue("@callingClass", logMessage.CallingClass);
                sqlCommand.Parameters.AddWithValue("@callingMethod", logMessage.CallingMethod);
                sqlCommand.ExecuteNonQuery();
            }
        }

        private string MakeInsertSqlCommand()
        {
            return string.Format(@"insert into {0} ([Text], [DateTime], [Level], [CallingClass], [CallingMethod]) 
                                   values (@text, @dateTime, @level, @callingClass, @callingMethod);", _tableName);
        }

        private void CreateTable()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var sqlCommand = new SqlCommand(@"
                        select object_name(object_id) as table_name from sys.objects
                        WHERE type_desc LIKE '%USER_TABLE' and lower(object_name(object_id)) like @tableName;", connection);

                sqlCommand.Parameters.AddWithValue("@tableName", _tableName.ToLower());
                var result = sqlCommand.ExecuteScalar();

                if (result == null)
                {
                    var commandText = string.Format(@"
                            create table [{0}]
                            (
	                            [Id] int not null primary key identity, 
                                [Text] nvarchar(max) null, 
                                [DateTime] datetime null, 
                                [Level] nvarchar(10) null, 
                                [CallingClass] nvarchar(500) NULL, 
                                [CallingMethod] nvarchar(500) NULL
                            );", _tableName);

                    sqlCommand = new SqlCommand(commandText, connection);
                    sqlCommand.ExecuteNonQuery();
                }
            }
        }
    }
}
