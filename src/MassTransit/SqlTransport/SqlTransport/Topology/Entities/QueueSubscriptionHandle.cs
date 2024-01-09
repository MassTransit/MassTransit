namespace MassTransit.SqlTransport.Topology
{
    using MassTransit.Topology;


    public interface QueueSubscriptionHandle :
        EntityHandle
    {
        TopicToQueueSubscription Subscription { get; }
    }
}
