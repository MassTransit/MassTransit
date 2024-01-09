namespace MassTransit.SqlTransport.Topology
{
    using MassTransit.Topology;


    public interface TopicSubscriptionHandle :
        EntityHandle
    {
        TopicToTopicSubscription Subscription { get; }
    }
}
