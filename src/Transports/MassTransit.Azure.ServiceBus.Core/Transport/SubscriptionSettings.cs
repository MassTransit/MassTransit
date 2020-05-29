namespace MassTransit.Azure.ServiceBus.Core.Transport
{
    using Microsoft.Azure.ServiceBus;
    using Microsoft.Azure.ServiceBus.Management;


    public interface SubscriptionSettings :
        ClientSettings
    {
        TopicDescription TopicDescription { get; }

        SubscriptionDescription SubscriptionDescription { get; }

        RuleDescription Rule { get; }

        Filter Filter { get; }

        bool RemoveSubscriptions { get; }
    }
}
