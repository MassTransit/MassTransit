using System;

namespace MassTransit.ServiceBus
{
    /// <summary>
    /// A non-generic interface for checking and delivering messages to a service bus handler
    /// </summary>
    public interface IMessageConsumer
    {
        /// <summary>
        /// Deliver the message to the handler
        /// </summary>
        /// <param name="bus">The service bus where the message arrived</param>
        /// <param name="envelope">The envelope containing the message</param>
        /// <param name="message">The message being delivered</param>
        void Deliver(IServiceBus bus, IEnvelope envelope, IMessage message);

        /// <summary>
        /// Allows the handler to determine if it will handle the message before retrieving it
        /// </summary>
        /// <param name="message">The message to handle</param>
        /// <returns>True if the message will be handled, otherwise false.</returns>
        bool IsHandled(IMessage message);
    }

    ///<summary>
    /// Implemented to handle the registration of handlers for a message type
    ///</summary>
    ///<typeparam name="T">The message type to handle</typeparam>
    public interface IMessageConsumer<T> : IMessageConsumer where T : IMessage
    {
        /// <summary>
        /// Adds a subscription to the message type for the specified handler
        /// </summary>
        /// <param name="callback">The function to call to handle the message</param>
        void Subscribe(MessageReceivedCallback<T> callback);

        /// <summary>
        /// Adds a subscription to the message type for the specified handler
        /// </summary>
        /// <param name="callback">The function to call to handle the message</param>
        /// <param name="condition">The condition function to determine if a message will be handled</param>
        void Subscribe(MessageReceivedCallback<T> callback, Predicate<T> condition);

        /// <summary>
        /// Deliver the message to the handler
        /// </summary>
        /// <param name="bus">The service bus where the message arrived</param>
        /// <param name="envelope">The envelope containing the message</param>
        /// <param name="message">The message being delivered</param>
        void Deliver(IServiceBus bus, IEnvelope envelope, T message);

        /// <summary>
        /// Allows the handler to determine if it will handle the message before retrieving it
        /// </summary>
        /// <param name="message">The message to handle</param>
        /// <returns>True if the message will be handled, otherwise false.</returns>
        bool IsHandled(T message);
    }

    /// <summary>
    /// The delegate for a message consumer
    /// </summary>
    /// <typeparam name="T">The type of message being delivered to the consumer</typeparam>
    /// <param name="ctx">The context of the received message</param>
    public delegate void MessageReceivedCallback<T>(MessageContext<T> ctx) where T : IMessage;
}