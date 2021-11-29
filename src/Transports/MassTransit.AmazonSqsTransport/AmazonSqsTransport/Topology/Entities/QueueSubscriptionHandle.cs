namespace MassTransit.AmazonSqsTransport.Topology
{
    using MassTransit.Topology;


    public interface QueueSubscriptionHandle :
        EntityHandle
    {
        QueueSubscription QueueSubscription { get; }
    }
}
