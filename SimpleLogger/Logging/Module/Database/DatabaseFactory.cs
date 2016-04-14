using MySql.Data.MySqlClient;
using Oracle.DataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace SimpleLogger.Logging.Module.Database
{
    public static class DatabaseFactory
    {
        public static DbConnection GetConnection(DatabaseType databaseType, string connectionString)
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

        public static DbCommand GetCommand(DatabaseType databaseType, string commandText, DbConnection connection)
        {
            switch (databaseType)
            {
                case DatabaseType.MsSql:
                    return new SqlCommand(commandText, connection as SqlConnection);
                case DatabaseType.Oracle:
                    return new OracleCommand(commandText, connection as OracleConnection);
                case DatabaseType.MySql:
                    return new MySqlCommand(commandText, connection as MySqlConnection);
            }

            return null;
        }
    }
}
