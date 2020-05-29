namespace MassTransit.Azure.ServiceBus.Core.Topology.Entities
{
    using MassTransit.Topology.Entities;


    public interface SubscriptionHandle :
        EntityHandle
    {
        Subscription Subscription { get; }
    }
}
