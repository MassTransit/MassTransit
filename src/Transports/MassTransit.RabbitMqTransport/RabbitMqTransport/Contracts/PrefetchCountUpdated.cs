namespace MassTransit.RabbitMqTransport.Contracts
{
    using System;


    /// <summary>
    /// Published/Returned when the prefetch count of a receive endpoint is updated
    /// </summary>
    public interface PrefetchCountUpdated
    {
        /// <summary>
        /// The time the prefetch count was updated
        /// </summary>
        DateTime Timestamp { get; }

        /// <summary>
        /// The name of the queue that was updated
        /// </summary>
        string QueueName { get; }

        /// <summary>
        /// The new prefetch count of the receive endpoint
        /// </summary>
        ushort PrefetchCount { get; }
    }
}
