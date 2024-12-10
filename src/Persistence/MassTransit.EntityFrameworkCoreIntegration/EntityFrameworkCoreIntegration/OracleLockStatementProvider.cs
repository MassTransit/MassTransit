namespace MassTransit.EntityFrameworkCoreIntegration
{
    public class OracleLockStatementProvider :
        SqlLockStatementProvider
    {
        public OracleLockStatementProvider(bool enableSchemaCaching = true)
            : base(new OracleLockStatementFormatter(), enableSchemaCaching)
        {
        }

        public OracleLockStatementProvider(string schemaName, bool enableSchemaCaching = true)
            : base(schemaName, new OracleLockStatementFormatter(), enableSchemaCaching)
        {
        }
    }
}
