namespace MassTransit.ActiveMqTransport.Topology
{
    public interface BrokerTopology :
        IProbeSite
    {
        Topic[] Topics { get; }
        Queue[] Queues { get; }
        Consumer[] Consumers { get; }
    }
}
