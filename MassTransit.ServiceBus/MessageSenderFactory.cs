using MassTransit.ServiceBus.Exceptions;

namespace MassTransit.ServiceBus
{
    using System;

    public class MessageSenderFactory
    {
        public static IMessageSender Create(IEndpoint endpoint)
        {
            if (endpoint is IMessageQueueEndpoint)
            {
                return new MessageQueueSender(endpoint as IMessageQueueEndpoint);
            }

            throw new EndpointException(endpoint, "No Message Sender Available");
        }
        public static IMessageSender Create(Uri endpoint)
        {
            return new MessageQueueSender(new MessageQueueEndpoint(endpoint));
        }
    }
}