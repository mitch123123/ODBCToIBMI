using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestDb2App.Helpers
{
    public class ConnectionHelper
    {
        public static string GetDB2ConnectionString()
        {
            string connString = Environment.GetEnvironmentVariable("DB2_CONNECTION", EnvironmentVariableTarget.User) ??
                             Environment.GetEnvironmentVariable("DB2_CONNECTION", EnvironmentVariableTarget.Machine);

            if (string.IsNullOrEmpty(connString))
            {
                throw new InvalidOperationException("❌ Error: Environment variable 'DB2_CONNECTION' is not set.");
            }

            return connString;
        }
    }
}
