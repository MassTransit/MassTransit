namespace MassTransit.Configuration
{
    /// <summary>
    /// Used internally by the Dapper provider to choose appropriate implementations of support objects.
    /// </summary>
    public enum DatabaseProviders
    {
        Unspecified,
        SqlServer,
        Postgres
    }
}
