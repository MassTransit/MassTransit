namespace MassTransit
{
    using System;


    public interface ISqlQueueEndpointConfigurator :
        ISqlQueueConfigurator
    {
        /// <summary>
        /// The polling interval of the queue when notifications are not available (or trusted)
        /// </summary>
        TimeSpan PollingInterval { set; }

        /// <summary>
        /// The message lock duration (set higher for longer-running consumers)
        /// </summary>
        TimeSpan LockDuration { set; }

        /// <summary>
        /// The maximum time a message can remain locked before being released for redelivery by the transport (up to MaxDeliveryCount)
        /// </summary>
        TimeSpan MaxLockDuration { set; }

        /// <summary>
        /// If true, messages that exist in the queue will be purged when the bus is started
        /// </summary>
        bool PurgeOnStartup { set; }

        /// <summary>
        /// Set the number of rows to process at a time when performing queue maintenance
        /// </summary>
        int MaintenanceBatchSize { set; }

        /// <summary>
        /// If true, expired messages will be moved to the dead letter queue instead of deleted
        /// </summary>
        bool DeadLetterExpiredMessages { set; }
    }
}
