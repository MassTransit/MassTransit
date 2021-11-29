namespace MassTransit.AzureServiceBusTransport.Topology
{
    public class SendEndpointBrokerTopologyBuilder :
        BrokerTopologyBuilder,
        ISendEndpointBrokerTopologyBuilder
    {
        public QueueHandle Queue { get; set; }
    }
}
