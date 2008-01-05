using System;

namespace MassTransit.ServiceBus
{
    public interface IMessageConsumer<T> where T : IMessage
    {
        void Subscribe(MessageReceivedCallback<T> callback);

        void Subscribe(MessageReceivedCallback<T> callback, Predicate<T> condition);
    }

    public delegate void MessageReceivedCallback<T>(MessageContext<T> ctx) where T : IMessage;
}