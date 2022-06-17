namespace MassTransit
{
    using System.Data;
    using EntityFrameworkCoreIntegration;


    public class EntityFrameworkOutboxOptions
    {
        public IsolationLevel IsolationLevel { get; set; } = IsolationLevel.Serializable;
        public ILockStatementProvider LockStatementProvider { get; set; } = new SqlServerLockStatementProvider();
    }
}
