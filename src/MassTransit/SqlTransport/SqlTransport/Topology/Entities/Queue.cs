namespace MassTransit.SqlTransport.Topology
{
    using System;


    public interface Queue
    {
        string QueueName { get; }

        /// <summary>
        /// Idle time before queue should be deleted (consumer-idle, not producer)
        /// </summary>
        TimeSpan? AutoDeleteOnIdle { get; }

        /// <summary>
        /// Specify the maximum delivery count for messages in the queue
        /// </summary>
        int? MaxDeliveryCount { get; }
    }
}
