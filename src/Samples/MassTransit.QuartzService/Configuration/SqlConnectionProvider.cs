namespace MassTransit.QuartzService.Configuration
{
    using System.Data;
    using System.Data.SqlClient;


    public class SqlConnectionProvider :
        IConnectionProvider
    {
        string _connectionString;

        public SqlConnectionProvider(IConnectionStringProvider connectionStringProvider, string connectionName)
        {
            _connectionString = connectionStringProvider.GetConnectionString(connectionName);
        }

        public IDbConnection GetConnection()
        {
            return new SqlConnection(_connectionString);
        }

        public IDbCommand GetCommand(string sql)
        {
            var connection = new SqlConnection(_connectionString);
            connection.Open();

            var command = new SqlCommand(sql, connection);

            return command;
        }
    }
}