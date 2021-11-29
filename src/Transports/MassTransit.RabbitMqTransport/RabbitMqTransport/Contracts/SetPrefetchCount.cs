namespace MassTransit.RabbitMqTransport.Contracts
{
    using System;


    /// <summary>
    /// Set the prefetch count of a receive endpoint
    /// </summary>
    public interface SetPrefetchCount
    {
        /// <summary>
        /// The time at which the change was requested
        /// </summary>
        DateTime Timestamp { get; }

        /// <summary>
        /// An optional queue name that if specified limits the setting to the queue name specified
        /// </summary>
        string QueueName { get; }

        /// <summary>
        /// The new prefetch count for the receive endpoint
        /// </summary>
        ushort PrefetchCount { get; }
    }
}
