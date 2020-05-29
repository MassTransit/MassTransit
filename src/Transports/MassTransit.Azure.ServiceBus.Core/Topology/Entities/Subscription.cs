namespace MassTransit.Azure.ServiceBus.Core.Topology.Entities
{
    using Microsoft.Azure.ServiceBus;
    using Microsoft.Azure.ServiceBus.Management;


    /// <summary>
    /// A subscription, as defined
    /// </summary>
    public interface Subscription
    {
        SubscriptionDescription SubscriptionDescription { get; }

        TopicHandle Topic { get; }

        RuleDescription Rule { get; }

        Filter Filter { get; }
    }
}
