namespace MassTransit.EntityFrameworkIntegration
{
    public class SqlServerLockStatementProvider :
        SqlLockStatementProvider
    {
        const string DefaultRowLockStatement = "SELECT 1 FROM {0}.{1} WITH (UPDLOCK, ROWLOCK) WHERE CorrelationId = @p0";
        const string DefaultSchemaName = "dbo";

        public SqlServerLockStatementProvider(bool enableSchemaCaching = true)
            : base(DefaultSchemaName, DefaultRowLockStatement, enableSchemaCaching)
        {
        }
    }
}
