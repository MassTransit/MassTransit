namespace MassTransit.AmazonSqsTransport.Topology
{
    /// <summary>
    /// A builder for creating the topology when publishing a message
    /// </summary>
    public interface ISendEndpointBrokerTopologyBuilder :
        IBrokerTopologyBuilder
    {
        /// <summary>
        /// The exchange to which the message is published
        /// </summary>
        QueueHandle Queue { get; }
    }
}
