using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleLogger.Logging.Module.Database
{
    public enum DatabaseType
    {
        MsSql = 1,
        Oracle = 2, // Only works on x64 machines
        MySql = 3
    }
}
