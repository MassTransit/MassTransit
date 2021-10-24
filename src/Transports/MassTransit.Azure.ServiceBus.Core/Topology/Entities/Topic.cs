namespace MassTransit.Azure.ServiceBus.Core.Topology.Entities
{
    using global::Azure.Messaging.ServiceBus.Administration;


    /// <summary>
    /// The exchange details used to declare the exchange to Azure Service Bus
    /// </summary>
    public interface Topic
    {
        CreateTopicOptions TopicDescription { get; }
    }
}
