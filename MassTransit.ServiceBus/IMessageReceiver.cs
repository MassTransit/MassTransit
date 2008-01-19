using System;

namespace MassTransit.ServiceBus
{
    public interface IMessageReceiverFactory
    {
        /// <summary>
        /// Creates an instance of an object that implements IMessageReceiver appropriate for the specified endpoint
        /// </summary>
        /// <param name="endpoint">The endpoint for receiving messages</param>
        /// <returns>An instance supporting IMessageReceiver</returns>
        IMessageReceiver Using(IEndpoint endpoint);
    }
}

namespace MassTransit.ServiceBus
{
    /// <summary>
    /// Used to begin receiving messages on an endpoint
    /// </summary>
    public interface IMessageReceiver :
        IDisposable
    {
        /// <summary>
        /// Adds a consumer to the message receiver
        /// </summary>
        /// <param name="consumer">The consumer to add</param>
        void Subscribe(IEnvelopeConsumer consumer);
    }
}