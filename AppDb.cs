using System;
using MySql.Data.MySqlClient;

namespace FirstDotNetCore
{
    public class AppDb : IDisposable
    {
        public MySqlConnection Connection;

        public AppDb(string connectionString = "server=47.74.235.241;user id=root;password=rootp@ssw0rd;port=3306;database=dotnetcore;")
        {
            Connection = new MySqlConnection(connectionString);
        }

        public void Dispose()
        {
            Connection.Close();
        }
    }
}
