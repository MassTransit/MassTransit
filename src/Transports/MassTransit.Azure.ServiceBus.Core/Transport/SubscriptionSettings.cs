namespace MassTransit.Azure.ServiceBus.Core.Transport
{
    using global::Azure.Messaging.ServiceBus.Administration;


    public interface SubscriptionSettings :
        ClientSettings
    {
        CreateTopicOptions CreateTopicOptions { get; }

        CreateSubscriptionOptions CreateSubscriptionOptions { get; }

        CreateRuleOptions Rule { get; }

        RuleFilter Filter { get; }

        bool RemoveSubscriptions { get; }
    }
}
