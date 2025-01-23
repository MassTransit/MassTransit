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
        /// The maximum number of message delivery attempts by the transport before moving the message to the DLQ
        /// </summary>
        int MaxDeliveryCount { set; }

        /// <summary>
        /// If true, messages that exist in the queue will be purged when the bus is started
        /// </summary>
        bool PurgeOnStartup { set; }

        /// <summary>
        /// Set the number of rows to process at a time when performing queue maintenance
        /// </summary>
        int MaintenanceBatchSize { set; }
    }
}
