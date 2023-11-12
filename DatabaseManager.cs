using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public SqlDataReader Get(string sql)
        {
            SqlCommand command = new SqlCommand(sql, GetConnection());
            SqlDataReader reader = command.ExecuteReader();

            return reader;
        }

        public SqlDataReader PreparedGet(string sql, params SqlParameter[] sqlParameter)
        {
            SqlCommand cmd = new SqlCommand(sql, GetConnection());

            foreach (SqlParameter param in sqlParameter)
            {
                cmd.Parameters.Add(param);
            }

            cmd.CommandType = CommandType.Text;

            SqlDataReader reader = cmd.ExecuteReader();

            return reader;
        }
    }
}
