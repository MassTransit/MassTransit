namespace MassTransit
{
    using System.Data;
    using EntityFrameworkCoreIntegration;


    public class EntityFrameworkOutboxOptions
    {
        public IsolationLevel IsolationLevel { get; set; } = IsolationLevel.RepeatableRead;
        public ILockStatementProvider LockStatementProvider { get; set; } = new SqlServerLockStatementProvider();
    }
}
