namespace MassTransit.Azure.ServiceBus.Core.Topology.Entities
{
    using MassTransit.Topology.Entities;


    public interface QueueSubscriptionHandle :
        EntityHandle
    {
        QueueSubscription QueueSubscription { get; }
    }
}
