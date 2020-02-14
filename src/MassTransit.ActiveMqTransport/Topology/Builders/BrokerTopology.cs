namespace MassTransit.ActiveMqTransport.Topology.Builders
{
    using Entities;
    using GreenPipes;


    public interface BrokerTopology :
        IProbeSite
    {
        Topic[] Topics { get; }
        Queue[] Queues { get; }
        Consumer[] Consumers { get; }
    }
}
