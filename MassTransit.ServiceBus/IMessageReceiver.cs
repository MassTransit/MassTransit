using System;

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