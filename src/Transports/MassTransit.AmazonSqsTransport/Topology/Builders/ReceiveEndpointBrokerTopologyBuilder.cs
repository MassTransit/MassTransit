namespace MassTransit.AmazonSqsTransport.Topology.Builders
{
    using Entities;


    public class ReceiveEndpointBrokerTopologyBuilder :
        BrokerTopologyBuilder,
        IReceiveEndpointBrokerTopologyBuilder
    {
        public QueueHandle Queue { get; set; }

        public BrokerTopology BuildTopologyLayout()
        {
            return new AmazonSqsBrokerTopology(Topics, Queues, QueueSubscriptions, TopicSubscriptions);
        }
    }
}
