namespace MassTransit.EventHubIntegration.Contexts
{
    using System;
    using Azure.Messaging.EventHubs.Producer;
    using GreenPipes;


    public interface ConnectionContext :
        PipeContext,
        IAsyncDisposable
    {
        IHostSettings HostSettings { get; }
        IStorageSettings StorageSettings { get; }

        EventHubProducerClient CreateEventHubClient(string eventHubName);
    }
}
