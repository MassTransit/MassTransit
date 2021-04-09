using EventStore.Client;

namespace MassTransit.EventStoreDbIntegration
{
    public delegate EventStoreClient EventStoreClientFactory();
}
