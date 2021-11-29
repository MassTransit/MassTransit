namespace MassTransit
{
    using System;


    /// <summary>
    /// Configures a queue/exchange pair in RabbitMQ
    /// </summary>
    public interface IRabbitMqQueueConfigurator :
        IRabbitMqExchangeConfigurator
    {
        /// <summary>
        /// Specify that the queue is exclusive to this process and cannot be accessed by other processes
        /// at the same time.
        /// </summary>
        bool Exclusive { set; }

        /// <summary>
        /// Sets the queue to be lazy (using less memory)
        /// </summary>
        bool Lazy { set; }

        /// <summary>
        /// Set the queue to expire after the specified time
        /// </summary>
        TimeSpan? QueueExpiration { set; }

        /// <summary>
        /// Allows to have only one consumer at a time consuming from a queue
        /// and to fail over to another registered consumer in case the active one is cancelled or dies
        /// </summary>
        bool SingleActiveConsumer { set; }

        /// <summary>
        /// Set a queue argument passed to the broker on queue declaration
        /// </summary>
        /// <param name="key">The argument key</param>
        /// <param name="value">The argument value</param>
        void SetQueueArgument(string key, object value);

        /// <summary>
        /// Set the queue argument to the TimeSpan (which is converted to milliseconds)
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        void SetQueueArgument(string key, TimeSpan value);

        /// <summary>
        /// Enable the message priority for the queue, specifying the maximum priority available
        /// </summary>
        /// <param name="maxPriority"></param>
        void EnablePriority(byte maxPriority);

        /// <summary>
        /// Specify that the queue should be a quorum queue
        /// </summary>
        /// <param name="replicationFactor">
        /// Optional, if specified must be greater than zero and less than the number of cluster nodes.
        /// An odd value is recommended.
        /// </param>
        void SetQuorumQueue(int? replicationFactor = default);
    }
}
