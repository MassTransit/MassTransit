namespace MassTransit.AzureServiceBusTransport
{
    using Azure.Messaging.ServiceBus.Administration;


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
