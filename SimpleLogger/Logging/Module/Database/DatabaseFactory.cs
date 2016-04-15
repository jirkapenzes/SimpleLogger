using MySql.Data.MySqlClient;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace SimpleLogger.Logging.Module.Database
{
    internal static class DatabaseFactory
    {
        internal static DbConnection GetConnection(DatabaseType databaseType, string connectionString)
        {
            switch (databaseType)
            {
                case DatabaseType.MsSql:
                    return new SqlConnection(connectionString);
                case DatabaseType.Oracle:
                    return new OracleConnection(connectionString);
                case DatabaseType.MySql:
                    return new MySqlConnection(connectionString);
            }

            return null;
        }

        internal static DbCommand GetCommand(DatabaseType databaseType, string commandText, DbConnection connection)
        {
            switch (databaseType)
            {
                case DatabaseType.MsSql:
                    return new SqlCommand(commandText, connection as SqlConnection);
                case DatabaseType.Oracle:
                    return new OracleCommand(commandText, connection as OracleConnection) { BindByName = true };
                case DatabaseType.MySql:
                    return new MySqlCommand(commandText, connection as MySqlConnection);
            }

            return null;
        }

        internal static string GetDatabaseName(DatabaseType databaseType)
        {
            switch (databaseType)
            {
                case DatabaseType.MsSql:
                    return "MsSqlDatabaseLoggerModule";
                case DatabaseType.Oracle:
                    return "OracleDatabaseLoggerModule";
                case DatabaseType.MySql:
                    return "MySqlDatabaseLoggerModule";
            }

            return string.Empty;
        }

        internal static string GetCreateTableQuery(DatabaseType databaseType)
        {
            switch (databaseType)
            {
                case DatabaseType.MsSql:
                    return @"create table [{0}]
                            (
	                            [Id] int not null primary key identity, 
                                [Text] nvarchar(4000) null, 
                                [DateTime] datetime null, 
                                [Log_Level] nvarchar(10) null, 
                                [CallingClass] nvarchar(500) NULL, 
                                [CallingMethod] nvarchar(500) NULL
                            );";
                case DatabaseType.Oracle:
                    return @"create table {0}
                                (
                                 Id int not null primary key, 
                                   Text varchar2(4000) null, 
                                   DateTime date null, 
                                   Log_Level varchar2(10) null, 
                                   CallingClass varchar2(500) NULL, 
                                   CallingMethod varchar2(500) NULL
                                );
                                create sequence seq_log nocache;";
                case DatabaseType.MySql:
                    return @"create table {0}
                            (
	                            Id int not null auto_increment,
                                Text varchar(4000) null, 
                                DateTime datetime null, 
                                Log_Level varchar(10) null, 
                                CallingClass varchar(500) NULL, 
                                CallingMethod varchar(500) NULL,
                                PRIMARY KEY (Id)
                            );";
            }

            return string.Empty;
        }

        internal static string GetCheckIfShouldCreateTableQuery(DatabaseType databaseType)
        {
            switch (databaseType)
            {
                case DatabaseType.MsSql:
                    return @"SELECT object_name(object_id) as table_name 
                               FROM sys.objects
                              WHERE type_desc LIKE '%USER_TABLE' 
                                AND lower(object_name(object_id)) like @tableName;";
                case DatabaseType.Oracle:
                    return @"SELECT TABLE_NAME 
                               FROM ALL_TABLES 
                              WHERE LOWER(TABLE_NAME) LIKE :tableName";
                case DatabaseType.MySql:
                    return @"SELECT table_name
                               FROM information_schema.tables
                              WHERE LOWER(table_name) = @tableName;";
            }

            return string.Empty;
        }

        internal static string GetInsertCommand(DatabaseType databaseType, string tableName)
        {
            switch (databaseType)
            {
                case DatabaseType.MsSql:
                    return string.Format(@"insert into {0} ([Text], [DateTime], [Log_Level], [CallingClass], [CallingMethod]) 
                                           values (@text, @dateTime, @log_level, @callingClass, @callingMethod);", tableName);
                case DatabaseType.Oracle:
                    return string.Format(@"insert into {0} (Id, Text, DateTime, Log_Level, CallingClass, CallingMethod) 
                                           values (seq_log.nextval, :text, :dateTime, :log_level, :callingClass, :callingMethod)", tableName);
                case DatabaseType.MySql:
                    return string.Format(@"insert into {0} (Text, DateTime, Log_Level, CallingClass, CallingMethod) 
                                           values (@text, @dateTime, @log_level, @callingClass, @callingMethod);", tableName);
            }

            return string.Empty;
        }
    }
}
