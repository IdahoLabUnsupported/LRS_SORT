using System.Data;
using System.Data.SqlClient;

namespace EmployeLink
{
    public class Config
    {
        public static string ConnectionString { get; set; }

        public static IDbConnection EmployeeConn => new SqlConnection(ConnectionString);
    }
}
