using System;
using System.Configuration;
using System.Data.SqlClient;

namespace BookstoreDBMS.DAL
{
    public class DatabaseHelper
    {
        private static readonly string _connectionString;

        static DatabaseHelper()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["BookstoreDB"]?.ConnectionString
                ?? throw new InvalidOperationException("Connection string 'BookstoreDB' not found in configuration.");
        }

        public static string ConnectionString => _connectionString;

        public static SqlConnection GetConnection()
        {
            var connection = new SqlConnection(_connectionString);
            connection.Open();
            return connection;
        }

        public static bool TestConnection()
        {
            try
            {
                using (var connection = GetConnection())
                {
                    return connection.State == System.Data.ConnectionState.Open;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}