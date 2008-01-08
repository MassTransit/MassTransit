using MassTransit.ServiceBus.Exceptions;

namespace MassTransit.ServiceBus
{
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
    }
}