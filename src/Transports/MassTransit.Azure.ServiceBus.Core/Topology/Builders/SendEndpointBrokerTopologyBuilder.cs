namespace MassTransit.Azure.ServiceBus.Core.Topology.Builders
{
    using Entities;


    public class SendEndpointBrokerTopologyBuilder :
        BrokerTopologyBuilder,
        ISendEndpointBrokerTopologyBuilder
    {
        public QueueHandle Queue { get; set; }
    }
}
