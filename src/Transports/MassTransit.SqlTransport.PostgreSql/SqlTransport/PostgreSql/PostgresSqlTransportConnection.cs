namespace MassTransit.SqlTransport.PostgreSql;

using System;
using System.Threading;
using System.Threading.Tasks;
using Npgsql;


public class PostgresSqlTransportConnection :
    IPostgresSqlTransportConnection
{
    public PostgresSqlTransportConnection(NpgsqlConnection connection)
    {
        Connection = connection;
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

        return new PostgresSqlTransportConnection(new NpgsqlConnection(builder.ToString()));
    }

    public static PostgresSqlTransportConnection GetDatabaseAdminConnection(SqlTransportOptions options)
    {
        var builder = CreateBuilder(options);

        if (!string.IsNullOrWhiteSpace(options.AdminUsername))
            builder.Username = options.AdminUsername;
        if (!string.IsNullOrWhiteSpace(options.AdminPassword))
            builder.Password = options.AdminPassword;

        return new PostgresSqlTransportConnection(new NpgsqlConnection(builder.ToString()));
    }

    public static PostgresSqlTransportConnection GetDatabaseConnection(SqlTransportOptions options)
    {
        return new PostgresSqlTransportConnection(new NpgsqlConnection(CreateBuilder(options).ToString()));
    }

    public static NpgsqlConnectionStringBuilder CreateBuilder(SqlTransportOptions options)
    {
        var builder = new NpgsqlConnectionStringBuilder(options.ConnectionString);

        if (!string.IsNullOrWhiteSpace(options.Host))
            builder.Host = options.Host;
        else if (!string.IsNullOrWhiteSpace(builder.Host))
            options.Host = builder.Host;

        if (!string.IsNullOrWhiteSpace(options.Database))
            builder.Database = options.Database;
        else if (!string.IsNullOrWhiteSpace(builder.Database))
            options.Database = builder.Database;

        if (!string.IsNullOrWhiteSpace(options.Username))
            builder.Username = options.Username;
        else if (!string.IsNullOrWhiteSpace(builder.Username))
            options.Username = builder.Username;

        if (!string.IsNullOrWhiteSpace(options.Password))
            builder.Password = options.Password;
        else if (!string.IsNullOrWhiteSpace(builder.Password))
            options.Password = builder.Password;

        if (options.Port.HasValue)
            builder.Port = options.Port.Value;
        else if (builder.Port != NpgsqlConnection.DefaultPort)
            options.Port = builder.Port;

        if (string.IsNullOrWhiteSpace(options.Schema))
            options.Schema = "transport";

        if (string.IsNullOrWhiteSpace(options.Role))
            options.Role = "transport";

        return builder;
    }

    public static string? GetAdminMigrationPrincipal(SqlTransportOptions options)
    {
        var principal = options.AdminUsername ?? options.Username ?? "postgres";

        return principal.Contains("@")
            ? principal.Substring(0, principal.IndexOf("@", StringComparison.Ordinal))
            : principal;
    }
}
