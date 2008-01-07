using MassTransit.ServiceBus.Exceptions;

namespace MassTransit.ServiceBus
{
    public class MessageSenderFactory
    {
        public static IMessageSender Create(IEndpoint endpoint)
        {
            if (endpoint is MessageQueueEndpoint)
            {
                return new MessageQueueSender(endpoint);
            }

            throw new EndpointException(endpoint, "No Message Sender Available");
        }
    }
}