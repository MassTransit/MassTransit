using MassTransit.ServiceBus.Exceptions;

namespace MassTransit.ServiceBus
{
    public class MessageReceiverFactory
    {
        public static IMessageReceiver Create(IEndpoint endpoint)
        {
            if (endpoint is IMessageQueueEndpoint)
            {
                return new MessageQueueReceiver(endpoint as IMessageQueueEndpoint);
            }

            throw new EndpointException(endpoint, "No Message Receiver Available. The endpoint is not of type 'IMessageQueueEndpoint'");
        }
    }
}