namespace MassTransit.Azure.ServiceBus.Core.Topology.Entities
{
    using global::Azure.Messaging.ServiceBus.Administration;


    /// <summary>
    /// The queue details used to declare the queue to Azure Service Bus
    /// </summary>
    public interface Queue
    {
        CreateQueueOptions CreateQueueOptions { get; }
    }
}
