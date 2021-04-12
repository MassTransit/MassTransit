using EventStore.Client;

namespace MassTransit.EventStoreDbIntegration
{
    public interface IHostSettings
    {
        string ConnectionString { get; }
        string ConnectionName { get; }
        UserCredentials DefaultCredentials { get; }
    }
}
