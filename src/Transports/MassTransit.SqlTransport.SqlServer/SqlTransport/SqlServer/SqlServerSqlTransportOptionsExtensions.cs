namespace MassTransit.SqlTransport.SqlServer;

public static class SqlServerSqlTransportOptionsExtensions
{
    public static string? FormatDataSource(this SqlTransportOptions options)
    {
        return options.Port.HasValue ? $"{options.Host},{options.Port}" : options.Host;
    }
}
