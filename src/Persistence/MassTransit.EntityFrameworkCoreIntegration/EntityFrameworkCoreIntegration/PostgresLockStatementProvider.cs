namespace MassTransit.EntityFrameworkCoreIntegration
{
    public class PostgresLockStatementProvider :
        SqlLockStatementProvider
    {
        public PostgresLockStatementProvider(bool enableSchemaCaching = true)
            : base(null, new PostgresLockStatementFormatter(), enableSchemaCaching)
        {
        }

        public PostgresLockStatementProvider(string schemaName, bool enableSchemaCaching = true)
            : base(schemaName, new PostgresLockStatementFormatter(), enableSchemaCaching)
        {
        }
    }
}
