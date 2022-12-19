namespace MassTransit.EventHubIntegration
{
    using Azure.Messaging.EventHubs.Producer;


    public interface ConnectionContext :
        PipeContext
    {
        EventHubProducerClient CreateEventHubClient(string eventHubName);
    }
}
