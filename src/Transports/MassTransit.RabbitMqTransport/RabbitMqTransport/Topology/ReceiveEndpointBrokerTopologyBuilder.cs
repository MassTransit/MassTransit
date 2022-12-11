namespace MassTransit.RabbitMqTransport.Topology
{
    public class ReceiveEndpointBrokerTopologyBuilder :
        BrokerTopologyBuilder,
        IReceiveEndpointBrokerTopologyBuilder
    {
        public QueueHandle Queue { get; set; }

        public ExchangeHandle Exchange { get; set; }

        public ExchangeHandle BoundExchange { get; set; }
    }
}
