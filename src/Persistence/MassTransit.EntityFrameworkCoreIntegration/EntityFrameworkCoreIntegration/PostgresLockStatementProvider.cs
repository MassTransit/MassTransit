namespace MassTransit.EntityFrameworkCoreIntegration
{
    public class PostgresLockStatementProvider :
        SqlLockStatementProvider
    {
        const string DefaultSchemaName = "public";

        public PostgresLockStatementProvider(bool enableSchemaCaching = true)
            : base(DefaultSchemaName, new PostgresLockStatementFormatter(), enableSchemaCaching)
        {
        }

        public PostgresLockStatementProvider(string schemaName, bool enableSchemaCaching = true)
            : base(schemaName, new PostgresLockStatementFormatter(), enableSchemaCaching)
        {
        }
    }
}
