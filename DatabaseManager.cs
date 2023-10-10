using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoClasses
{
    internal class DatabaseManager
    {
        private static SqlConnection _connection = new SqlConnection(@"Server=mauvechemineeposeur.ddns.net;Database=bd1;User ID=sa;Password=<Minecraft123>;Encrypt=no");

        public static SqlConnection GetConnection()
        {
            if (_connection == null)
            {
                _connection = new SqlConnection();
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
