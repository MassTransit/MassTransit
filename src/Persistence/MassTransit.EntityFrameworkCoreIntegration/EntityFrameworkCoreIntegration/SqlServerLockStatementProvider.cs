namespace MassTransit.EntityFrameworkCoreIntegration
{
    public class SqlServerLockStatementProvider :
        SqlLockStatementProvider
    {
        public SqlServerLockStatementProvider(bool enableSchemaCaching = true, bool serializable = false)
            : base(new SqlServerLockStatementFormatter(serializable), enableSchemaCaching)
        {
        }

        public SqlServerLockStatementProvider(string schemaName, bool enableSchemaCaching = true, bool serializable = false)
            : base(schemaName, new SqlServerLockStatementFormatter(serializable), enableSchemaCaching)
        {
        }
    }
}
