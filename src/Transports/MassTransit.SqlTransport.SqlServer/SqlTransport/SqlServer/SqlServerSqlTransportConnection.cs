namespace MassTransit.SqlTransport.SqlServer
{
    using System.Data;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Data.SqlClient;


    public class SqlServerSqlTransportConnection :
        ISqlServerSqlTransportConnection
    {
        public SqlServerSqlTransportConnection(string connectionString)
        {
            Connection = new SqlConnection(connectionString);
        }

        public SqlConnection Connection { get; }

        public ValueTask DisposeAsync()
        {
            Connection.Dispose();

            return default;
        }

        IDbConnection ISqlTransportConnection.Connection => Connection;

        public Task Open(CancellationToken cancellationToken = default)
        {
            return Connection.OpenAsync(cancellationToken);
        }

        public Task Close()
        {
            Connection.Close();

            return Task.CompletedTask;
        }

        public SqlCommand CreateCommand(string commandText)
        {
            var command = new SqlCommand(commandText);
            command.Connection = Connection;

            return command;
        }

        public static SqlServerSqlTransportConnection GetSystemDatabaseConnection(SqlTransportOptions options)
        {
            var builder = new SqlConnectionStringBuilder
            {
                DataSource = FormatDataSource(options),
                InitialCatalog = "master",
                UserID = options.AdminUsername ?? options.Username,
                Password = options.AdminPassword ?? options.Password,
                TrustServerCertificate = true
            };


            return new SqlServerSqlTransportConnection(builder.ToString());
        }

        public static SqlServerSqlTransportConnection GetDatabaseAdminConnection(SqlTransportOptions options)
        {
            var builder = new SqlConnectionStringBuilder
            {
                DataSource = FormatDataSource(options),
                InitialCatalog = options.Database,
                UserID = options.AdminUsername ?? options.Username,
                Password = options.AdminPassword ?? options.Password,
                TrustServerCertificate = true
            };


            return new SqlServerSqlTransportConnection(builder.ToString());
        }

        public static SqlServerSqlTransportConnection GetDatabaseConnection(SqlTransportOptions options)
        {
            var builder = new SqlConnectionStringBuilder
            {
                DataSource = FormatDataSource(options),
                InitialCatalog = options.Database,
                UserID = options.Username,
                Password = options.Password,
                TrustServerCertificate = true
            };

            return new SqlServerSqlTransportConnection(builder.ToString());
        }

        static string? FormatDataSource(SqlTransportOptions options)
        {
            return options.Port.HasValue ? $"{options.Host},{options.Port}" : options.Host;
        }
    }
}
