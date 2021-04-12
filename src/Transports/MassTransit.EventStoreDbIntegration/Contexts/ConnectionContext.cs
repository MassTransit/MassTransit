using EventStore.Client;
using GreenPipes;

namespace MassTransit.EventStoreDbIntegration.Contexts
{
    public interface ConnectionContext
        : PipeContext
    {
        EventStoreClient CreateEventStoreDbClient();
    }
}
