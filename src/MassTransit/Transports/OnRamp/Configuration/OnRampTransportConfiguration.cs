using MassTransit.Transports.OnRamp.StatementProviders;

namespace MassTransit.Transports.OnRamp.Configuration
{
    public class OnRampTransportConfiguration : OnRampTransportOptions
    {
        /// <summary>
        /// Defaults to <see cref="HostNameInstanceIdGenerator" />
        /// </summary>
        public IInstanceIdGenerator InstanceIdGenerator { get; private set; } = new SimpleInstanceIdGenerator();

        public void SetInstanceId(IInstanceIdGenerator generator)
        {
            InstanceIdGenerator = generator;
        }

        public void SetInstanceId(string instanceId)
        {
            InstanceIdGenerator = new StaticInstanceIdGenerator(instanceId);
        }

        public IRepositoryNamingProvider RepositoryNamingProvider { get; private set; }

        public void UseRepositoryNamingProvider(IRepositoryNamingProvider namingProvider)
        {
            RepositoryNamingProvider = namingProvider;
        }

        public void UseSqlite()
        {
            RepositoryNamingProvider = new SqliteRepositoryNamingProvider("mt");
        }

        public void UseSqlServer()
        {
            RepositoryNamingProvider = new SqlServerRepositoryNamingProvider("mt");
        }

        public void UsePostgres()
        {
            RepositoryNamingProvider = new PostgresRepositoryNamingProvider("mt");
        }
    }
}
