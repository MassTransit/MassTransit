namespace MassTransit.AmazonSqsTransport.Topology.Builders
{
    using Entities;
    using GreenPipes;


    public interface BrokerTopology :
        IProbeSite
    {
        Topic[] Topics { get; }
        Queue[] Queues { get; }
        QueueSubscription[] QueueSubscriptions { get; }
        TopicSubscription[] TopicSubscriptions { get; }
    }
}
