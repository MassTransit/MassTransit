namespace MassTransit
{
    /// <summary>
    /// Configures a queue/exchange pair in RabbitMQ
    /// </summary>
    public interface IQueueConfigurator
    {
        /// <summary>
        /// Specify the maximum number of concurrent messages that are consumed
        /// </summary>
        /// <value>The limit</value>
        ushort PrefetchCount { set; }

        /// <summary>
        /// Specify the queue should be durable (survives broker restart) or in-memory
        /// </summary>
        /// <param name="durable">True for a durable queue, False for an in-memory queue</param>
        void Durable(bool durable = true);

        /// <summary>
        /// Specify that the queue is exclusive to this process and cannot be accessed by other processes
        /// at the same time.
        /// </summary>
        /// <param name="exclusive">True for exclusive, otherwise false</param>
        void Exclusive(bool exclusive = true);

        /// <summary>
        /// Specify that the queue (and the exchange of the same name) should be created as auto-delete
        /// </summary>
        /// <param name="autoDelete"></param>
        void AutoDelete(bool autoDelete = true);

        /// <summary>
        /// Specify the exchange type for the endpoint
        /// </summary>
        /// <param name="exchangeType"></param>
        void ExchangeType(string exchangeType);

        /// <summary>
        /// Purge the messages from an existing queue on startup (note that upon reconnection to the server
        /// the queue will not be purged again, only when the service is restarted).
        /// </summary>
        /// <param name="purgeOnStartup"></param>
        void PurgeOnStartup(bool purgeOnStartup = true);

        /// <summary>
        /// Set a queue argument passed to the broker on queue declaration
        /// </summary>
        /// <param name="key">The argument key</param>
        /// <param name="value">The argument value</param>
        void SetQueueArgument(string key, object value);

        /// <summary>
        /// Set an exchange argument passed to the broker on queue declaration
        /// </summary>
        /// <param name="key">The argument key</param>
        /// <param name="value">The argument value</param>
        void SetExchangeArgument(string key, object value);
    }
}