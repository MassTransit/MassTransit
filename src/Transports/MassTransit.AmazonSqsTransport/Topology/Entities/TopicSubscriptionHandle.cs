namespace MassTransit.AmazonSqsTransport.Topology.Entities
{
    using MassTransit.Topology.Entities;


    public interface TopicSubscriptionHandle :
        EntityHandle
    {
        TopicSubscription TopicSubscription { get; }
    }
}
