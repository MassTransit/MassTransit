namespace MassTransit.SqlTransport.PostgreSql.Helpers;

public static class NotifyChannel
{
    // Postgres channel name is an identifier and can contain maximum 63 characters.
    // We need to reserve 19 characters for queueId being a long and
    // 5 characters for the _msg_ separator. Schema can be at most 39 characters.
    // https://www.postgresql.org/docs/current/sql-syntax-lexical.html#SQL-SYNTAX-IDENTIFIERS
    const int MaxSchemaNameLength = 39;
    const string DefaultSchemaName = "transport";

    public static string SanitizeSchemaName(string? schemaName)
    {
        if (string.IsNullOrWhiteSpace(schemaName))
        {
            return DefaultSchemaName;
        }

        return schemaName!.Length > MaxSchemaNameLength
            ? schemaName.Substring(0, MaxSchemaNameLength)
            : schemaName;
    }
}
