using EventStore.Client;
using GreenPipes;

namespace MassTransit.EventStoreDbIntegration.Contexts
{
    public interface ConnectionContext
        : PipeContext
    {
        IHostSettings HostSettings { get; }

        EventStoreClient CreateEventStoreDbClient();
    }
}
