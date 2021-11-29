namespace MassTransit
{
    using System;


    /// <summary>
    /// Configures an exchange for RabbitMQ
    /// </summary>
    public interface IRabbitMqExchangeConfigurator
    {
        /// <summary>
        /// Specify the queue should be durable (survives broker restart) or in-memory
        /// </summary>
        /// <value>True for a durable queue, False for an in-memory queue</value>
        bool Durable { set; }

        /// <summary>
        /// Specify that the queue (and the exchange of the same name) should be created as auto-delete
        /// </summary>
        bool AutoDelete { set; }

        /// <summary>
        /// Specify the exchange type for the endpoint
        /// </summary>
        string ExchangeType { set; }

        /// <summary>
        /// Set an exchange argument passed to the broker on queue declaration
        /// </summary>
        /// <param name="key">The argument key</param>
        /// <param name="value">The argument value</param>
        void SetExchangeArgument(string key, object value);

        /// <summary>
        /// Set the exchange argument to the TimeSpan (which is converted to milliseconds)
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        void SetExchangeArgument(string key, TimeSpan value);
    }
}
