namespace MassTransit.EventHubIntegration
{
    using Azure.Messaging.EventHubs.Producer;
    using Configuration;


    public interface ConnectionContext :
        PipeContext
    {
        IHostSettings HostSettings { get; }
        IStorageSettings StorageSettings { get; }

        EventHubProducerClient CreateEventHubClient(string eventHubName);
    }
}
