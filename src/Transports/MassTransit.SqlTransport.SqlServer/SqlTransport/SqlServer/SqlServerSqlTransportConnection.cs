namespace MassTransit.SqlTransport.SqlServer;

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

    public Task Open(CancellationToken cancellationToken = default)
    {
        return Connection.OpenAsync(cancellationToken);
    }

    public Task Close()
    {
        Connection.Close();

        return Task.CompletedTask;
    }

    public static SqlServerSqlTransportConnection GetSystemDatabaseConnection(SqlTransportOptions options)
    {
        var builder = CreateBuilder(options);

        builder.InitialCatalog = "master";

        if (!string.IsNullOrWhiteSpace(options.AdminUsername))
            builder.UserID = options.AdminUsername;
        if (!string.IsNullOrWhiteSpace(options.AdminPassword))
            builder.Password = options.AdminPassword;

        return new SqlServerSqlTransportConnection(builder.ToString());
    }

    public static SqlServerSqlTransportConnection GetDatabaseAdminConnection(SqlTransportOptions options)
    {
        var builder = CreateBuilder(options);

        if (!string.IsNullOrWhiteSpace(options.AdminUsername))
            builder.UserID = options.AdminUsername;
        if (!string.IsNullOrWhiteSpace(options.AdminPassword))
            builder.Password = options.AdminPassword;

        return new SqlServerSqlTransportConnection(builder.ToString());
    }

    public static SqlServerSqlTransportConnection GetDatabaseConnection(SqlTransportOptions options)
    {
        var builder = CreateBuilder(options);

        return new SqlServerSqlTransportConnection(builder.ToString());
    }

    public static SqlConnectionStringBuilder CreateBuilder(SqlTransportOptions options)
    {
        var builder = new SqlConnectionStringBuilder(options.ConnectionString) { TrustServerCertificate = true };

        if (!string.IsNullOrWhiteSpace(options.Host))
            builder.DataSource = options.FormatDataSource();
        else if (!string.IsNullOrWhiteSpace(builder.DataSource))
            (options.Host, options.Port) = ParseDataSource(builder.DataSource);

        if (!string.IsNullOrWhiteSpace(options.Database))
            builder.InitialCatalog = options.Database;
        else if (!string.IsNullOrWhiteSpace(builder.InitialCatalog))
            options.Database = builder.InitialCatalog;

        if (!string.IsNullOrWhiteSpace(options.Username))
            builder.UserID = options.Username;
        else if (!string.IsNullOrWhiteSpace(builder.UserID))
            options.Username = builder.UserID;
        if (!string.IsNullOrWhiteSpace(options.Password))
            builder.Password = options.Password;
        else if (!string.IsNullOrWhiteSpace(builder.Password))
            options.Password = builder.Password;

        if (string.IsNullOrWhiteSpace(options.Schema))
            options.Schema = "transport";

        if (string.IsNullOrWhiteSpace(options.Role))
            options.Role = "transport";

        return builder;
    }

    static (string? host, int? port) ParseDataSource(string? source)
    {
        var split = source?.Split(',');
        if (split?.Length == 2)
        {
            var host = split[0].Trim();

            if (int.TryParse(split[1].Trim(), out var port))
                return (host, port);

            return (host, null);
        }

        return (source?.Trim(), null);
    }
}
