using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace Tennis
{
    public class DatabaseManager
    {
        private static SqlConnection _connection;

        public static SqlConnection GetConnection()
        {
            if (_connection == null)
            {
                string connectionString = ConfigurationManager.ConnectionStrings["ChemineeDB"].ConnectionString;
                _connection = new SqlConnection(connectionString);
            }

            if (_connection.State == ConnectionState.Open)
            {
                return _connection;
            }

            _connection.Open();

            return _connection;
        }
    }
}
