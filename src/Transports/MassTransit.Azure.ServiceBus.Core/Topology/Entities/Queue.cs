namespace MassTransit.Azure.ServiceBus.Core.Topology.Entities
{
    using Microsoft.Azure.ServiceBus.Management;


    /// <summary>
    /// The queue details used to declare the queue to Azure Service Bus
    /// </summary>
    public interface Queue
    {
        QueueDescription QueueDescription { get; }
    }
}
