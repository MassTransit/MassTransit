namespace MassTransit.RabbitMqTransport
{
    using System;
    using Topology;


    public interface SendSettings :
        EntitySettings
    {
        /// <summary>
        /// Returns the send address for the settings
        /// </summary>
        /// <param name="hostAddress"></param>
        /// <returns></returns>
        RabbitMqEndpointAddress GetSendAddress(Uri hostAddress);

        /// <summary>
        /// Return the BrokerTopology to apply at startup (to create exchange and queue if binding is specified)
        /// </summary>
        /// <returns></returns>
        BrokerTopology GetBrokerTopology();
    }
}
