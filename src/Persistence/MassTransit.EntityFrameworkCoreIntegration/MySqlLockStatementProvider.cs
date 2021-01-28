namespace MassTransit.EntityFrameworkCoreIntegration
{
    public class MySqlLockStatementProvider :
        SqlLockStatementProvider
    {
        const string DefaultSchemaName = "public";
        const string DefaultRowLockStatement = "SELECT * FROM {1} WHERE {2} = \"@p0\" FOR UPDATE";

        public MySqlLockStatementProvider(bool enableSchemaCaching = true)
            : base(DefaultSchemaName, DefaultRowLockStatement, enableSchemaCaching)
        {
        }
    }
}
