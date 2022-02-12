namespace MassTransit
{
    using System;
    using Configuration;


    public interface IPublishTopology :
        IPublishTopologyConfigurationObserverConnector
    {
        /// <summary>
        /// Returns the specification for the message type
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        /// <returns></returns>
        IMessagePublishTopology<T> GetMessageTopology<T>()
            where T : class;

        /// <summary>
        /// Returns the publish address for the message, using the topology rules. This cannot use
        /// a PublishContext because the transport isn't available yet.
        /// </summary>
        /// <param name="messageType">The message type</param>
        /// <param name="baseAddress">The host base address, used to build out the exchange address</param>
        /// <param name="publishAddress">The address where the publish endpoint should send the message</param>
        /// <returns>true if the address was available, otherwise false</returns>
        bool TryGetPublishAddress(Type messageType, Uri baseAddress, out Uri? publishAddress);
    }
}
