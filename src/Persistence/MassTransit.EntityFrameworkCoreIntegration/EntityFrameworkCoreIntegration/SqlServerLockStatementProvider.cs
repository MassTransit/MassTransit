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
    }
}
