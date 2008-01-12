using System;

namespace MassTransit.ServiceBus
{
    /// <summary>
    /// Used to send envelopes to an endpoint
    /// </summary>
    public interface IMessageSender :
        IDisposable
    {
        /// <summary>
        /// Sends the envelope to the endpoint attached to the object implementing <c ref="IMessageSender" />
        /// </summary>
        /// <param name="envelope">The envelope to send</param>
        void Send(IEnvelope envelope);
    }
}