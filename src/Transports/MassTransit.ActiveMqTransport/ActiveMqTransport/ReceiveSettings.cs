namespace MassTransit.ActiveMqTransport
{
    using System;


    /// <summary>
    /// Specify the receive settings for a receive transport
    /// </summary>
    public interface ReceiveSettings :
        EntitySettings
    {
        /// <summary>
        /// The number of unacknowledged messages to allow to be processed concurrently
        /// </summary>
        int PrefetchCount { get; }

        int ConcurrentMessageLimit { get; }

        string Selector { get; }

        /// <summary>
        /// Get the input address for the transport on the specified host
        /// </summary>
        Uri GetInputAddress(Uri hostAddress);
    }
}
