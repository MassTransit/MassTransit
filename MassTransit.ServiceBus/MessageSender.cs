using System;
using MassTransit.ServiceBus.Exceptions;

namespace MassTransit.ServiceBus
{
    /// <summary>
    /// An abstract factory to create an IMessageSender
    /// </summary>
    public class MessageSender
    {
        /// <summary>
        /// Using an IMessageSender using the specified endpoint
        /// </summary>
        /// <param name="endpoint">The endpoint for sending messages</param>
        /// <returns>An instance that supports IMessageSender</returns>
        public static IMessageSender Using(IEndpoint endpoint)
        {
            if (endpoint is IMessageQueueEndpoint)
            {
                return new MessageQueueSender(endpoint as IMessageQueueEndpoint);
            }

            throw new EndpointException(endpoint, "No Message Sender Available for " + endpoint.Uri);
        }

        /// <summary>
        /// Using an IMessageSender using the specified endpoint
        /// </summary>
        /// <param name="endpoint">The endpoint for sending messages</param>
        /// <returns>An instance that supports IMessageSender</returns>
        public static IMessageSender Using(Uri endpoint)
        {
            return new MessageQueueSender(new MessageQueueEndpoint(endpoint));
        }
    }
}