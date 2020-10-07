namespace MassTransit.EntityFrameworkCoreIntegration
{
    public class PostgresLockStatementProvider :
        SqlLockStatementProvider
    {
        const string DefaultSchemaName = "public";
        const string DefaultRowLockStatement = "SELECT * FROM \"{0}\".\"{1}\" WHERE \"{2}\" = @p0 FOR UPDATE";

        public PostgresLockStatementProvider(bool enableSchemaCaching = true)
            : base(DefaultSchemaName, DefaultRowLockStatement, enableSchemaCaching)
        {
        }
    }
}
