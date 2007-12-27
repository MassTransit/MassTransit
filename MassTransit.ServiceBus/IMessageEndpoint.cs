using System;

namespace MassTransit.ServiceBus
{
    public interface IMessageEndpoint<T> : 
		IEndpoint where T : IMessage
    {
        event EventHandler<MessageReceivedEventArgs<T>> MessageReceived;
    }
}