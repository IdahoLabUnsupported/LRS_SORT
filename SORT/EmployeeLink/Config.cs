using System.Data;
using System.Data.SqlClient;

namespace EmployeeLink
{
    public class Config
    {
        public static string DefaultConnectionString { get; set; }
        public static string EmployeeConnectionString { get; set; }

        public static IDbConnection EmployeeConn => new SqlConnection(EmployeeConnectionString);
        public static IDbConnection DefaultConn => new SqlConnection(DefaultConnectionString);

    }
}
