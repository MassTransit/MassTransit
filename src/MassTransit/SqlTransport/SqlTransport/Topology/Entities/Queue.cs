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
    }
}
