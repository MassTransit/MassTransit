using EventStore.Client;
using GreenPipes;

namespace MassTransit.EventStoreDbIntegration.Contexts
{
    public interface ClientContext
        : PipeContext
    {
        IHostSettings HostSettings { get; }

        EventStoreClient CreateEventStoreDbClient();
    }
}
