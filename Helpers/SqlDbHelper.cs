using Npgsql;

namespace uts_habits.Helpers
{
    public class SqlDbHelper : IDisposable
    {
        private NpgsqlConnection _connection;
        private string _connectionString;
        public SqlDbHelper(string pConnectionString)
        {
            _connectionString = pConnectionString;
            _connection = new NpgsqlConnection(_connectionString);
        }
        public void OpenConnection()
        {
            if (_connection.State != System.Data.ConnectionState.Open)
            {
                _connection.Open();
            }
        }
        public NpgsqlCommand NpgsqlCommand(string pQuery)
        {
            this.OpenConnection();
            return new NpgsqlCommand(pQuery, _connection);
        }
        public void CloseConnection()
        {
            if (_connection.State != System.Data.ConnectionState.Closed)
            {
                _connection.Close();
            }
        }
        public void Dispose()
        {
            this.CloseConnection();
            _connection.Dispose();
        }
    }
}