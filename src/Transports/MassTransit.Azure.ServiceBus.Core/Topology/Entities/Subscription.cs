namespace MassTransit.Azure.ServiceBus.Core.Topology.Entities
{
    using global::Azure.Messaging.ServiceBus.Administration;


    /// <summary>
    /// A subscription, as defined
    /// </summary>
    public interface Subscription
    {
        CreateSubscriptionOptions SubscriptionDescription { get; }

        TopicHandle Topic { get; }

        CreateRuleOptions Rule { get; }

        RuleFilter Filter { get; }
    }
}
