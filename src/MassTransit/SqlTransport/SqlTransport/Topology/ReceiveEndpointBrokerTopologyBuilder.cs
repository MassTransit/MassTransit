namespace MassTransit.SqlTransport.Topology
{
    public class ReceiveEndpointBrokerTopologyBuilder :
        BrokerTopologyBuilder,
        IReceiveEndpointBrokerTopologyBuilder
    {
        public ReceiveEndpointBrokerTopologyBuilder(ReceiveSettings settings)
        {
            Queue = CreateQueue(settings.QueueName, settings.AutoDeleteOnIdle, settings.MaxDeliveryCount);
        }

        public QueueHandle Queue { get; }
    }
}
