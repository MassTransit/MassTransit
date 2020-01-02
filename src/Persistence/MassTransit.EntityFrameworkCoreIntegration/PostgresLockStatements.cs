namespace MassTransit.EntityFrameworkCoreIntegration
{
    public class PostgresLockStatements : RawSqlLockStatements
    {
        const string defaultSchema = "public";
        const string rowLockStatement = "SELECT * FROM \"{0}\".\"{1}\" WHERE \"CorrelationId\" = @p0 FOR UPDATE";

        public PostgresLockStatements(bool enableSchemaCaching = true) : base(defaultSchema, rowLockStatement, enableSchemaCaching)
        {
        }
    }
}
