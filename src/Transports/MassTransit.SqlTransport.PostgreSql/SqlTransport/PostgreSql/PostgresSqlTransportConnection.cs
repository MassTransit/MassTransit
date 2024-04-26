namespace MassTransit.SqlTransport.PostgreSql
{
    using System;
    using System.Data;
    using System.Threading;
    using System.Threading.Tasks;
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
            var builder = CreateBuilder(options);

            builder.Database = "postgres";

            if (!string.IsNullOrWhiteSpace(options.AdminUsername))
                builder.Username = options.AdminUsername;
            if (!string.IsNullOrWhiteSpace(options.AdminPassword))
                builder.Password = options.AdminPassword;

            return new PostgresSqlTransportConnection(builder.ToString());
        }

        public static PostgresSqlTransportConnection GetDatabaseAdminConnection(SqlTransportOptions options)
        {
            var builder = CreateBuilder(options);

            if (!string.IsNullOrWhiteSpace(options.AdminUsername))
                builder.Username = options.AdminUsername;
            if (!string.IsNullOrWhiteSpace(options.AdminPassword))
                builder.Password = options.AdminPassword;

            return new PostgresSqlTransportConnection(builder.ToString());
        }

        public static PostgresSqlTransportConnection GetDatabaseConnection(SqlTransportOptions options)
        {
            return new PostgresSqlTransportConnection(CreateBuilder(options).ToString());
        }

        public static NpgsqlConnectionStringBuilder CreateBuilder(SqlTransportOptions options)
        {
            var builder = new NpgsqlConnectionStringBuilder(options.ConnectionString);

            if (!string.IsNullOrWhiteSpace(options.Host))
                builder.Host = options.Host;
            if (!string.IsNullOrWhiteSpace(options.Database))
                builder.Database = options.Database;
            if (!string.IsNullOrWhiteSpace(options.Username))
                builder.Username = options.Username;
            if (!string.IsNullOrWhiteSpace(options.Password))
                builder.Password = options.Password;
            if (options.Port.HasValue)
                builder.Port = options.Port.Value;

            return builder;
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
