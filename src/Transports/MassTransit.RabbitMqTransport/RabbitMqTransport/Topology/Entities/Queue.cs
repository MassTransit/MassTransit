namespace MassTransit.RabbitMqTransport.Topology
{
    using System.Collections.Generic;


    /// <summary>
    /// The queue details used to declare the queue to RabbitMQ
    /// </summary>
    public interface Queue
    {
        /// <summary>
        /// The queue name
        /// </summary>
        string QueueName { get; }

        /// <summary>
        /// True if the queue should be durable, and survive a broker restart
        /// </summary>
        bool Durable { get; }

        /// <summary>
        /// True if the queue should be deleted when the connection is closed
        /// </summary>
        bool AutoDelete { get; }

        /// <summary>
        /// True if the queue should be exclusive and not shared
        /// </summary>
        bool Exclusive { get; }

        /// <summary>
        /// Additional queue arguments
        /// </summary>
        IDictionary<string, object> QueueArguments { get; }
    }
}
