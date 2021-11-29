namespace MassTransit.AzureServiceBusTransport.Topology
{
    using Azure.Messaging.ServiceBus.Administration;


    /// <summary>
    /// A subscription, as defined
    /// </summary>
    public interface Subscription
    {
        CreateSubscriptionOptions CreateSubscriptionOptions { get; }

        TopicHandle Topic { get; }

        CreateRuleOptions Rule { get; }

        RuleFilter Filter { get; }
    }
}
