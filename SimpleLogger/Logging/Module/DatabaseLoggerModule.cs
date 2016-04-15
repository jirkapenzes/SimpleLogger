using SimpleLogger.Logging.Module.Database;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace SimpleLogger.Logging.Module
{
    public class DatabaseLoggerModule : LoggerModule
    {
        private readonly string _connectionString;
        private readonly string _tableName;
        private readonly DatabaseType _databaseType;

        public DatabaseLoggerModule(DatabaseType databaseType, string connectionString) 
            : this(databaseType, connectionString, "Log") { }

        public DatabaseLoggerModule(DatabaseType databaseType, string connectionString, string tableName)
        {
            _databaseType = databaseType;
            _connectionString = connectionString;
            _tableName = tableName;
        }

        public override void Initialize()
        {
            CreateTable();
        }

        public override string Name
        {
            get { return DatabaseFactory.GetDatabaseName(_databaseType); }
        }

        private DbParameter GetParameter(DbCommand command, string name, object value, DbType type)
        {
            var parameter = command.CreateParameter();
            parameter.DbType = type;
            parameter.ParameterName = (_databaseType.Equals(DatabaseType.Oracle) ? ":" : "@") + name;
            parameter.Value = value;
            return parameter;
        }

        private void AddParameter(DbCommand command, string name, object value, DbType type)
        {
            command.Parameters.Add(GetParameter(command, name, value, type));
        }

        public override void AfterLog(LogMessage logMessage)
        {
            using (var connection = DatabaseFactory.GetConnection(_databaseType, _connectionString))
            {
                connection.Open();
                var commandText = DatabaseFactory.GetInsertCommand(_databaseType, _tableName);
                var sqlCommand = DatabaseFactory.GetCommand(_databaseType, commandText, connection);

                AddParameter(sqlCommand, "text", logMessage.Text, DbType.String);
                AddParameter(sqlCommand, "dateTime", logMessage.DateTime, DbType.Date);
                AddParameter(sqlCommand, "log_level", logMessage.Level.ToString(), DbType.String);
                AddParameter(sqlCommand, "callingClass", logMessage.CallingClass, DbType.String);
                AddParameter(sqlCommand, "callingMethod", logMessage.CallingMethod, DbType.String);

                sqlCommand.ExecuteNonQuery();
            }
        }

        private void CreateTable()
        {
            var connection = DatabaseFactory.GetConnection(_databaseType, _connectionString);

            using (connection)
            {
                connection.Open();
                var sqlCommand = DatabaseFactory.GetCommand
                (
                    _databaseType,
                    DatabaseFactory.GetCheckIfShouldCreateTableQuery(_databaseType), 
                    connection
                );

                AddParameter(sqlCommand, "tableName", _tableName.ToLower(), DbType.String);

                var result = sqlCommand.ExecuteScalar();

                if (result == null)
                {
                    var commandText = string.Format(DatabaseFactory.GetCreateTableQuery(_databaseType), _tableName);
                    sqlCommand = DatabaseFactory.GetCommand(_databaseType, commandText, connection);
                    sqlCommand.ExecuteMultipleNonQuery();
                }
            }
        }
    }
}
