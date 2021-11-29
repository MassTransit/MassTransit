namespace MassTransit.EntityFrameworkCoreIntegration
{
    public class SqlServerLockStatementProvider :
        SqlLockStatementProvider
    {
        const string DefaultRowLockStatement = "SELECT * FROM {0}.{1} WITH (UPDLOCK, ROWLOCK) WHERE {2} = @p0";
        const string DefaultSchemaName = "dbo";

        public SqlServerLockStatementProvider(bool enableSchemaCaching = true)
            : base(DefaultSchemaName, DefaultRowLockStatement, enableSchemaCaching)
        {
        }
    }
}
