namespace MassTransit.ServiceBus
{
    using System;

    public interface IMessageSenderFactory
    {
        /// <summary>
        /// Using an IMessageSender using the specified endpoint
        /// </summary>
        /// <param name="endpoint">The endpoint for sending messages</param>
        /// <returns>An instance that supports IMessageSender</returns>
        IMessageSender Using(IEndpoint endpoint);

        /// <summary>
        /// Using an IMessageSender using the specified endpoint
        /// </summary>
        /// <param name="endpoint">The endpoint for sending messages</param>
        /// <returns>An instance that supports IMessageSender</returns>
        IMessageSender Using(Uri endpoint);
    }
}