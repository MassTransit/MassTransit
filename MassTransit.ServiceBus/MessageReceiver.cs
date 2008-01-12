using MassTransit.ServiceBus.Exceptions;

namespace MassTransit.ServiceBus
{
    /// <summary>
    /// An abstract factory to create an IMessageReceiver
    /// </summary>
    public class MessageReceiver
    {
        /// <summary>
        /// Creates an instance of an object that implements IMessageReceiver appropriate for the specified endpoint
        /// </summary>
        /// <param name="endpoint">The endpoint for receiving messages</param>
        /// <returns>An instance supporting IMessageReceiver</returns>
        public static IMessageReceiver Using(IEndpoint endpoint)
        {
            if (endpoint is IMessageQueueEndpoint)
            {
                return new MessageQueueReceiver(endpoint as IMessageQueueEndpoint);
            }

            throw new EndpointException(endpoint, "No Message Receiver Available. The endpoint is not of type 'IMessageQueueEndpoint'");
        }
    }
}