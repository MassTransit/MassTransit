namespace MassTransit.ActiveMqTransport.Topology
{
    public class ReceiveEndpointBrokerTopologyBuilder :
        BrokerTopologyBuilder,
        IReceiveEndpointBrokerTopologyBuilder
    {
        public QueueHandle Queue { get; set; }

        public BrokerTopology BuildTopologyLayout()
        {
            return new ActiveMqBrokerTopology(Topics, Queues, Consumers);
        }
    }
}
