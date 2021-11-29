namespace MassTransit.AzureServiceBusTransport.Topology
{
    public class SubscriptionEndpointBrokerTopologyBuilder :
        BrokerTopologyBuilder,
        ISubscriptionEndpointBrokerTopologyBuilder
    {
        public TopicHandle Topic { get; set; }
    }
}
