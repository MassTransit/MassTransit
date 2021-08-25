using MassTransit.Transports.Outbox.StatementProviders;

namespace MassTransit.Transports.Outbox.Configuration
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

        public IRepositoryNamingProvider RepositoryNamingProvider { get; private set; } = new RepositoryNamingProvider("mt");

        public void UseRepositoryNamingProvider(string schema)
        {
            RepositoryNamingProvider = new RepositoryNamingProvider(schema);
        }

        public void UseRepositoryNamingProvider(IRepositoryNamingProvider namingProvider)
        {
            RepositoryNamingProvider = namingProvider;
        }
    }
}
