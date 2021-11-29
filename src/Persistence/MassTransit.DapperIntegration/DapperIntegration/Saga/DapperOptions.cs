namespace MassTransit.DapperIntegration.Saga
{
    using System.Data;


    public class DapperOptions<TSaga>
    {
        public DapperOptions(string connectionString, IsolationLevel isolationLevel)
        {
            ConnectionString = connectionString;
            IsolationLevel = isolationLevel;
        }

        public string ConnectionString { get; }
        public IsolationLevel IsolationLevel { get; }
    }
}
