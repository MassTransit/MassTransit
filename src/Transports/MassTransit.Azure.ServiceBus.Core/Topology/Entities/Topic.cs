namespace MassTransit.Azure.ServiceBus.Core.Topology.Entities
{
    using Microsoft.Azure.ServiceBus.Management;


    /// <summary>
    /// The exchange details used to declare the exchange to Azure Service Bus
    /// </summary>
    public interface Topic
    {
        TopicDescription TopicDescription { get; }
    }
}
