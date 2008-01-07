using MassTransit.ServiceBus.Exceptions;

namespace MassTransit.ServiceBus
{
    public class MessageReceiverFactory
    {
        public static IMessageReceiver Create(IEndpoint endpoint)
        {
            if (endpoint is MessageQueueEndpoint)
            {
                return new MessageQueueReceiver(endpoint);
            }

            throw new EndpointException(endpoint, "No Message Receiver Available");
        }
    }
}