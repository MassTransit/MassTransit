namespace MassTransit.AzureServiceBusTransport.Topology
{
    using Azure.Messaging.ServiceBus.Administration;


    /// <summary>
    /// The queue details used to declare the queue to Azure Service Bus
    /// </summary>
    public interface Queue
    {
        CreateQueueOptions CreateQueueOptions { get; }
    }
}
