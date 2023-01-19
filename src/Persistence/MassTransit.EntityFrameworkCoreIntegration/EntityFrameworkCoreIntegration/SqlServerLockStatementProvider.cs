namespace MassTransit.EntityFrameworkCoreIntegration
{
    public class SqlServerLockStatementProvider :
        SqlLockStatementProvider
    {
        const string DefaultSchemaName = "dbo";

        public SqlServerLockStatementProvider(bool enableSchemaCaching = true)
            : base(DefaultSchemaName, new SqlServerLockStatementFormatter(), enableSchemaCaching)
        {
        }

        public SqlServerLockStatementProvider(string schemaName, bool enableSchemaCaching = true)
            : base(schemaName, new SqlServerLockStatementFormatter(), enableSchemaCaching)
        {
        }
    }
}
