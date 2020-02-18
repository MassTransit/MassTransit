namespace MassTransit.RabbitMqTransport.Topology.Builders
{
    using Entities;


    public class ReceiveEndpointBrokerTopologyBuilder :
        BrokerTopologyBuilder,
        IReceiveEndpointBrokerTopologyBuilder
    {
        public QueueHandle Queue { get; set; }

        public ExchangeHandle Exchange { get; set; }
    }
}
