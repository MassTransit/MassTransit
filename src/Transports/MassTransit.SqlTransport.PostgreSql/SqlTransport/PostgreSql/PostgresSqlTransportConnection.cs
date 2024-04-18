namespace MassTransit.SqlTransport.PostgreSql
{
    using System;
    using System.Data;
    using System.Threading;
    using System.Threading.Tasks;
    using MassTransit.SqlTransport;
    using Npgsql;


    public class PostgresSqlTransportConnection :
        IPostgresSqlTransportConnection
    {
        public PostgresSqlTransportConnection(string connectionString)
        {
            Connection = new NpgsqlConnection(connectionString);
        }

        public ValueTask DisposeAsync()
        {
            return Connection.DisposeAsync();
        }

        public NpgsqlConnection Connection { get; }

        public NpgsqlCommand CreateCommand(string commandText)
        {
            var command = new NpgsqlCommand(commandText);
            command.Connection = Connection;

            return command;
        }

        IDbConnection ISqlTransportConnection.Connection => Connection;

        public Task Open(CancellationToken cancellationToken = default)
        {
            return Connection.OpenAsync(cancellationToken);
        }

        public Task Close()
        {
            return Connection.CloseAsync();
        }

        public static PostgresSqlTransportConnection GetSystemDatabaseConnection(SqlTransportOptions options)
        {
            var builder = new NpgsqlConnectionStringBuilder
            {
                Host = options.Host,
                Database = "postgres",
                Username = options.AdminUsername ?? options.Username,
                Password = options.AdminPassword ?? options.Password
            };

            if (options.Port.HasValue)
                builder.Port = options.Port.Value;

            return new PostgresSqlTransportConnection(builder.ToString());
        }

        public static PostgresSqlTransportConnection GetDatabaseAdminConnection(SqlTransportOptions options)
        {
            var builder = new NpgsqlConnectionStringBuilder
            {
                Host = options.Host,
                Database = options.Database,
                Username = options.AdminUsername ?? options.Username,
                Password = options.AdminPassword ?? options.Password
            };

            if (options.Port.HasValue)
                builder.Port = options.Port.Value;

            return new PostgresSqlTransportConnection(builder.ToString());
        }

        public static PostgresSqlTransportConnection GetDatabaseConnection(SqlTransportOptions options)
        {
            var builder = new NpgsqlConnectionStringBuilder
            {
                Host = options.Host,
                Database = options.Database,
                Username = options.Username,
                Password = options.Password
            };

            if (options.Port.HasValue)
                builder.Port = options.Port.Value;

            return new PostgresSqlTransportConnection(builder.ToString());
        }

        public static string? GetAdminMigrationPrincipal(SqlTransportOptions options)
        {
            var principal = options.AdminUsername ?? "postgres";

            principal = principal?.Contains("@") ?? false
                ? principal.Substring(0, principal.IndexOf("@", StringComparison.Ordinal))
                : principal;

            return principal;
        }
    }
}
