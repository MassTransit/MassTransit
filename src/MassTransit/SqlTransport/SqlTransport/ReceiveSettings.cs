namespace MassTransit.SqlTransport
{
    using System;


    /// <summary>
    /// Specify the receive settings for a receive transport
    /// </summary>
    public interface ReceiveSettings :
        EntitySettings
    {
        /// <summary>
        /// The queue name to receive from
        /// </summary>
        string QueueName { get; }

        /// <summary>
        /// Once the topology is configured, the queueId should be available
        /// </summary>
        long? QueueId { get; }

        /// <summary>
        /// The number of unacknowledged messages to allow to be processed concurrently
        /// </summary>
        int PrefetchCount { get; }

        int ConcurrentMessageLimit { get; }

        int ConcurrentDeliveryLimit { get; }

        SqlReceiveMode ReceiveMode { get; }

        /// <summary>
        /// If True, and a queue name is specified, if the queue exists and has messages, they are purged at startup
        /// If the connection is reset, messages are not purged until the service is reset
        /// </summary>
        bool PurgeOnStartup { get; }

        /// <summary>
        /// Message locks are automatically renewed, however, the actual lock duration determines how long the message remains locked
        /// when the consumer process crashes.
        /// </summary>
        TimeSpan LockDuration { get; }

        /// <summary>
        /// The maximum amount of time the lock will be renewed during message consumption before being abandoned
        /// </summary>
        TimeSpan MaxLockDuration { get; }

        /// <summary>
        /// The maximum number of message delivery attempts by the transport before moving the message to the DLQ (defaults to 10)
        /// </summary>
        int? MaxDeliveryCount { get; }

        /// <summary>
        /// How often to poll for messages when no messages exist
        /// </summary>
        TimeSpan PollingInterval { get; }

        /// <summary>
        /// The amount of time, when a message is abandoned, before the message is available for redelivery
        /// </summary>
        TimeSpan? UnlockDelay { get; }

        int MaintenanceBatchSize { get; }

        bool DeadLetterExpiredMessages { get; }
    }
}
