namespace MassTransit.AzureServiceBusTransport.Topology
{
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
