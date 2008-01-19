using System;
using MassTransit.ServiceBus.Exceptions;

namespace MassTransit.ServiceBus
{
    using Util;

    /// <summary>
    /// An abstract factory to create an IMessageSender
    /// </summary>
    public class MessageSenderFactory : IMessageSenderFactory
    {
        /// <summary>
        /// Using an IMessageSender using the specified endpoint
        /// </summary>
        /// <param name="endpoint">The endpoint for sending messages</param>
        /// <returns>An instance that supports IMessageSender</returns>
        public IMessageSender Using(IEndpoint endpoint)
        {
            Check.Require(endpoint is IMessageQueueEndpoint,
                          string.Format("Endpoint: {0} - is not of type {1} ", endpoint.Uri,
                                        typeof (IMessageQueueEndpoint).FullName));

            return new MessageQueueSender(endpoint as IMessageQueueEndpoint);
        }

        /// <summary>
        /// Using an IMessageSender using the specified endpoint
        /// </summary>
        /// <param name="endpoint">The endpoint for sending messages</param>
        /// <returns>An instance that supports IMessageSender</returns>
        public IMessageSender Using(Uri endpoint)
        {
            return new MessageQueueSender(new MessageQueueEndpoint(endpoint));
        }
    }
}