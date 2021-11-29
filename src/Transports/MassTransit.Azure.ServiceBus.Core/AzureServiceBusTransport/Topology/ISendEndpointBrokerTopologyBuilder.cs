namespace MassTransit.AzureServiceBusTransport.Topology
{
    /// <summary>
    /// A builder for creating the topology when publishing a message
    /// </summary>
    public interface ISendEndpointBrokerTopologyBuilder :
        IBrokerTopologyBuilder
    {
        /// <summary>
        /// The topic to which the message is published
        /// </summary>
        QueueHandle Queue { get; set; }
    }
}
