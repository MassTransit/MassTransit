using EventStore.Client;

namespace MassTransit.EventStoreDbIntegration
{
    public delegate ICheckpointStore CheckpointStoreFactory(EventStoreClient client);
}
