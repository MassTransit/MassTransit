#nullable enable
namespace MassTransit.DapperIntegration.Saga
{
    using System.Data;


    public class DapperOptions<TSaga>
        where TSaga : class, ISaga
    {
        public DapperOptions(string connectionString, IsolationLevel isolationLevel, DatabaseContextFactory<TSaga>? contextFactory)
        {
            ConnectionString = connectionString;
            IsolationLevel = isolationLevel;
            ContextFactory = contextFactory;
        }

        public string ConnectionString { get; }
        public IsolationLevel IsolationLevel { get; }
        public DatabaseContextFactory<TSaga>? ContextFactory { get; }
    }
}
