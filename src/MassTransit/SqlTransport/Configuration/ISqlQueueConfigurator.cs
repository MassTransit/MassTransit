namespace MassTransit
{
    using System;


    /// <summary>
    /// Configure a database transport queue
    /// </summary>
    public interface ISqlQueueConfigurator
    {
        /// <summary>
        /// If specified, the queue will be automatically removed after no consumer activity within the specific idle period
        /// </summary>
        TimeSpan? AutoDeleteOnIdle { set; }

        /// <summary>
        /// The maximum number of message delivery attempts by the transport before moving the message to the DLQ (defaults to 10)
        /// </summary>
        int? MaxDeliveryCount { set; }
    }
}
