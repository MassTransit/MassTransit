namespace MassTransit.EntityFrameworkCoreIntegration
{
    /// <summary>
    /// Provides default lock statements for MS SQL / SQL Server.
    ///
    /// If you need a custom default schema or a custom lock statement, use <see cref="RawSqlLockStatements"/> directly
    /// and pass appropriate values into the constructor.
    /// </summary>
    public class SqlServerLockStatements : RawSqlLockStatements
    {
        const string defaultSchema = "dbo";
        const string rowLockStatement = "SELECT * FROM {0}.{1} WITH (UPDLOCK, ROWLOCK) WHERE CorrelationId = @p0";

        public SqlServerLockStatements(bool enableSchemaCaching = true) : base(defaultSchema, rowLockStatement, enableSchemaCaching)
        {
        }
    }
}
