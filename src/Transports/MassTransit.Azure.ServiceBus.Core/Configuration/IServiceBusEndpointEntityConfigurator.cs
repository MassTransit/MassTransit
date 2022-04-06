namespace MassTransit
{
    using System;


    public interface IServiceBusEndpointEntityConfigurator :
        IServiceBusEntityConfigurator
    {
        /// <summary>
        /// Specify the lock duration for messages read from the queue
        /// </summary>
        TimeSpan? LockDuration { set; }

        /// <summary>
        /// Sets the maximum delivery count. A message is automatically dead lettered after this number of deliveries.
        /// </summary>
        int? MaxDeliveryCount { set; }

        /// <summary>
        /// Sets the subscription in session mode, requiring a session for inbound messages
        /// </summary>
        bool? RequiresSession { set; }

        /// <summary>
        /// Sets the maximum number of concurrent calls per session
        /// </summary>
        int? MaxConcurrentCallsPerSession { set; }

        /// <summary>
        /// Move messages to the dead letter queue on expiration (time to live exceeded)
        /// </summary>
        bool? EnableDeadLetteringOnMessageExpiration { set; }

        /// <summary>
        /// Sets the path to the recipient to which the dead lettered message is forwarded.
        /// </summary>
        string ForwardDeadLetteredMessagesTo { set; }
    }
}
