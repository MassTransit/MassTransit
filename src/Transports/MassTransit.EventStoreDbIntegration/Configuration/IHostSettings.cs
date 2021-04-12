using EventStore.Client;

namespace MassTransit.EventStoreDbIntegration
{
    public interface IHostSettings
    {
        bool UseExistingClient { get; }
        string ConnectionString { get; }
        string ConnectionName { get; }
        UserCredentials DefaultCredentials { get; }
    }
}
