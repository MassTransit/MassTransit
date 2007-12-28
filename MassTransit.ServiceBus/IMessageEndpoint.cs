using System;

namespace MassTransit.ServiceBus
{
    public interface IMessageEndpoint<T> : 
		IEndpoint where T : IMessage
    {
        event MessageHandler<T> MessageReceived;
    }

    public delegate void MessageHandler<T>(IServiceBus bus, IEnvelope envelope, T message);
}