namespace MassTransit.RabbitMqTransport
{
    using System.Collections.Generic;


    public interface EntitySettings
    {
        /// <summary>
        /// True if messages should be persisted to disk for the queue
        /// </summary>
        bool Durable { get; }

        /// <summary>
        /// True if the queue/exchange should automatically be deleted
        /// </summary>
        bool AutoDelete { get; }

        /// <summary>
        /// Arguments passed to exchange-declare
        /// </summary>
        IDictionary<string, object> ExchangeArguments { get; }

        /// <summary>
        /// The exchange name to bind to the queue as the default exchange
        /// </summary>
        string ExchangeName { get; }

        /// <summary>
        /// The RabbitMQ exchange type
        /// </summary>
        string ExchangeType { get; }
    }
}
