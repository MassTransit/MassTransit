namespace MassTransit.RabbitMqTransport.Topology
{
    /// <summary>
    /// A builder for creating the topology when publishing a message
    /// </summary>
    public interface IPublishEndpointBrokerTopologyBuilder :
        IBrokerTopologyBuilder
    {
        /// <summary>
        /// The exchange to which the message is published
        /// </summary>
        ExchangeHandle Exchange { get; set; }

        IPublishEndpointBrokerTopologyBuilder CreateImplementedBuilder();
    }
}
