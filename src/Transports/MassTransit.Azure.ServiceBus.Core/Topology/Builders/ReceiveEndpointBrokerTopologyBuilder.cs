namespace MassTransit.Azure.ServiceBus.Core.Topology.Builders
{
    using Entities;


    public class ReceiveEndpointBrokerTopologyBuilder :
        BrokerTopologyBuilder,
        IReceiveEndpointBrokerTopologyBuilder
    {
        public QueueHandle Queue { get; set; }
    }
}
