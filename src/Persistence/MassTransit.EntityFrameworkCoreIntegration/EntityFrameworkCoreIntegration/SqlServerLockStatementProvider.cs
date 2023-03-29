namespace MassTransit.EntityFrameworkCoreIntegration
{
    public class SqlServerLockStatementProvider :
        SqlLockStatementProvider
    {
        public SqlServerLockStatementProvider(bool enableSchemaCaching = true)
            : base(new SqlServerLockStatementFormatter(), enableSchemaCaching)
        {
        }

        public SqlServerLockStatementProvider(string schemaName, bool enableSchemaCaching = true)
            : base(schemaName, new SqlServerLockStatementFormatter(), enableSchemaCaching)
        {
        }
    }
}
