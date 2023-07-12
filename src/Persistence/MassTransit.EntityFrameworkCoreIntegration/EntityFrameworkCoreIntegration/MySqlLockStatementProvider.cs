namespace MassTransit.EntityFrameworkCoreIntegration
{
    public class MySqlLockStatementProvider :
        SqlLockStatementProvider
    {
        public MySqlLockStatementProvider(bool enableSchemaCaching = true)
            : base(new MySqlLockStatementFormatter(), enableSchemaCaching)
        {
        }
    }
}
