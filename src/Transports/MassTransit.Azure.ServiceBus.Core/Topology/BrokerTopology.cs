namespace MassTransit.Azure.ServiceBus.Core.Topology
{
    using Entities;
    using GreenPipes;


    public interface BrokerTopology :
        IProbeSite
    {
        Topic[] Topics { get; }
        Queue[] Queues { get; }
        Subscription[] Subscriptions { get; }
        QueueSubscription[] QueueSubscriptions { get; }
        TopicSubscription[] TopicSubscriptions { get; }
    }
}
