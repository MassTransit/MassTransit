namespace MassTransit.AzureServiceBusTransport.Topology
{
    /// <summary>
    /// A builder for creating the topology when publishing a message
    /// </summary>
    public interface IPublishEndpointBrokerTopologyBuilder :
        IBrokerTopologyBuilder
    {
        /// <summary>
        /// The topic to which the message is published
        /// </summary>
        TopicHandle Topic { get; set; }

        /// <summary>
        /// Create an implemented builder which can be passed to implemented types
        /// </summary>
        /// <returns></returns>
        IPublishEndpointBrokerTopologyBuilder CreateImplementedBuilder();
    }
}
