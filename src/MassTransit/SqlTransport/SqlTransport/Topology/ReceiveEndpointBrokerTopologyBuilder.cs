namespace MassTransit.SqlTransport.Topology
{
    public class ReceiveEndpointBrokerTopologyBuilder :
        BrokerTopologyBuilder,
        IReceiveEndpointBrokerTopologyBuilder
    {
        public ReceiveEndpointBrokerTopologyBuilder(ReceiveSettings settings)
        {
            Queue = CreateQueue(settings.QueueName, settings.AutoDeleteOnIdle);
        }

        public QueueHandle Queue { get; }
    }
}
