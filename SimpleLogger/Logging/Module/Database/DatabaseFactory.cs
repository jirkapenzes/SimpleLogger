using Oracle.DataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace SimpleLogger.Logging.Module.Database
{
    public class DatabaseFactory
    {
        public DbConnection GetConnection(DatabaseType databaseType, string connectionString)
        {
            switch (databaseType)
            {
                case DatabaseType.MsSql:
                    return new SqlConnection(connectionString);
                case DatabaseType.Oracle:
                    return new OracleConnection(connectionString);
            }

            return null;
        }

        public DbCommand GetCommand(DatabaseType databaseType, DbConnection connection, string commandText)
        {
            switch (databaseType)
            {
                case DatabaseType.MsSql:
                    return new SqlCommand(commandText, connection as SqlConnection);
                case DatabaseType.Oracle:
                    return new OracleCommand(commandText, connection as OracleConnection);
            }

            return null;
        }
    }
}
