namespace MassTransit
{
    using System;
    using Configuration;


    /// <summary>
    /// The message-specific publish topology, which may be configured or otherwise
    /// setup for use with the publish specification.
    /// </summary>
    /// <typeparam name="TMessage"></typeparam>
    public interface IMessagePublishTopology<TMessage> :
        IMessagePublishTopology
        where TMessage : class
    {
        void Apply(ITopologyPipeBuilder<PublishContext<TMessage>> builder);
    }


    public interface IMessagePublishTopology
    {
        /// <summary>
        /// Returns the publish address for the message, using the topology rules. This cannot use
        /// a PublishContext because the transport isn't available yet.
        /// </summary>
        /// <param name="baseAddress">The host base address, used to build out the exchange address</param>
        /// <param name="publishAddress">The address where the publish endpoint should send the message</param>
        /// <returns>true if the address was available, otherwise false</returns>
        bool TryGetPublishAddress(Uri baseAddress, out Uri? publishAddress);
    }
}
