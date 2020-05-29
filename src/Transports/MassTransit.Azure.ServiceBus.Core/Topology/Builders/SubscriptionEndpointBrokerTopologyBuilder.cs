namespace MassTransit.Azure.ServiceBus.Core.Topology.Builders
{
    using Entities;


    public class SubscriptionEndpointBrokerTopologyBuilder :
        BrokerTopologyBuilder,
        ISubscriptionEndpointBrokerTopologyBuilder
    {
        public TopicHandle Topic { get; set; }
    }
}
