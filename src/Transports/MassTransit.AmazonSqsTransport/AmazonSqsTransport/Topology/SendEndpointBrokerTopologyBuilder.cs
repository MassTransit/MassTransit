namespace MassTransit.AmazonSqsTransport.Topology
{
    public class SendEndpointBrokerTopologyBuilder :
        BrokerTopologyBuilder,
        ISendEndpointBrokerTopologyBuilder
    {
        /// <summary>
        /// The queue to which messages are sent
        /// </summary>
        public QueueHandle Queue { get; set; }

        public BrokerTopology BuildBrokerTopology()
        {
            return new AmazonSqsBrokerTopology(Topics, Queues, QueueSubscriptions, TopicSubscriptions);
        }
    }
}
