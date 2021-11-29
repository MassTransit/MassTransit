namespace MassTransit.AzureServiceBusTransport.Topology
{
    using Azure.Messaging.ServiceBus.Administration;


    /// <summary>
    /// The exchange details used to declare the exchange to Azure Service Bus
    /// </summary>
    public interface Topic
    {
        CreateTopicOptions CreateTopicOptions { get; }
    }
}
