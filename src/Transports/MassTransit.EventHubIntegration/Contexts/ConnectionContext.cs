namespace MassTransit.EventHubIntegration.Contexts
{
    using Azure.Messaging.EventHubs.Producer;
    using GreenPipes;


    public interface ConnectionContext :
        PipeContext
    {
        IHostSettings HostSettings { get; }
        IStorageSettings StorageSettings { get; }

        EventHubProducerClient CreateEventHubClient(string eventHubName);
    }
}
