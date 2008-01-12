using System;

namespace MassTransit.ServiceBus
{
    public interface IMessageConsumer<T> where T : IMessage
    {
        void Subscribe(MessageReceivedCallback<T> callback);

        void Subscribe(MessageReceivedCallback<T> callback, Predicate<T> condition);
    }

    /// <summary>
    /// The delegate for a message consumer
    /// </summary>
    /// <typeparam name="T">The type of message being delivered to the consumer</typeparam>
    /// <param name="ctx">The context of the received message</param>
    public delegate void MessageReceivedCallback<T>(MessageContext<T> ctx) where T : IMessage;
}