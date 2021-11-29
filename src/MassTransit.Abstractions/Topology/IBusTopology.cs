namespace MassTransit
{
    using System;


    public interface IBusTopology
    {
        IPublishTopology PublishTopology { get; }

        ISendTopology SendTopology { get; }

        /// <summary>
        /// Returns the publish topology for the specified message type
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        /// <returns></returns>
        IMessagePublishTopology<T> Publish<T>()
            where T : class;

        /// <summary>
        /// Returns the send topology for the specified message type
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        /// <returns></returns>
        IMessageSendTopology<T> Send<T>()
            where T : class;

        /// <summary>
        /// Returns the message topology for the specified message type
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        /// <returns></returns>
        IMessageTopology<T> Message<T>()
            where T : class;

        /// <summary>
        /// Returns the destination address for the specified message type, as a short address.
        /// </summary>
        /// <param name="messageType">The message type</param>
        /// <param name="publishAddress"></param>
        /// <returns></returns>
        bool TryGetPublishAddress(Type messageType, out Uri publishAddress);

        /// <summary>
        /// Returns the destination address for the specified message type, as a short address.
        /// </summary>
        /// <param name="publishAddress"></param>
        /// <returns></returns>
        bool TryGetPublishAddress<T>(out Uri publishAddress)
            where T : class;
    }
}
