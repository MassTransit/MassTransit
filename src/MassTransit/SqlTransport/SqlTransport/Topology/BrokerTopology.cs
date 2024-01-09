namespace MassTransit.SqlTransport.Topology
{
    public interface BrokerTopology :
        IProbeSite
    {
        Topic[] Topics { get; }
        Queue[] Queues { get; }
        TopicToTopicSubscription[] TopicSubscriptions { get; }
        TopicToQueueSubscription[] QueueSubscriptions { get; }
    }
}
