namespace MassTransit.RabbitMqTransport
{
    using System;


    /// <summary>
    /// Configures a queue/exchange pair in RabbitMQ
    /// </summary>
    public interface IQueueConfigurator :
        IExchangeConfigurator
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
    }
}
