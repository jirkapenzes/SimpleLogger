using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace SimpleLogger.Logging.Module.Database
{
    internal static class DatabaseExtensions
    {
        internal static void ExecuteMultipleNonQuery(this IDbCommand dbCommand)
        {
            var sqlStatementArray = dbCommand.CommandText
                                             .Split(new string[] { ";" },
                                                    StringSplitOptions.RemoveEmptyEntries);

            foreach (string sqlStatement in sqlStatementArray)
            {
                dbCommand.CommandText = sqlStatement;
                dbCommand.ExecuteNonQuery();
            }
        }
    }
}
