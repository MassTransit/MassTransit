namespace MassTransit.AzureServiceBusTransport
{
    using Topology;


    /// <summary>
    /// The settings for sending to an endpoint
    /// </summary>
    public interface SendSettings
    {
        /// <summary>
        /// The path of the messaging entity
        /// </summary>
        string EntityPath { get; }

        BrokerTopology GetBrokerTopology();
    }
}
