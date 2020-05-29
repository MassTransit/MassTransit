namespace MassTransit.RabbitMqTransport.Topology.Entities
{
    using System.Collections.Generic;


    /// <summary>
    /// The details of an exchange to be bound
    /// </summary>
    public interface ExchangeSettings
    {
        /// <summary>
        /// The name of the exchange
        /// </summary>
        string ExchangeName { get; }

        /// <summary>
        /// The exchange type (fanout,etc.)
        /// </summary>
        string ExchangeType { get; }

        /// <summary>
        /// True if the exchange should be durable, and survive a broker restart
        /// </summary>
        bool Durable { get; }

        /// <summary>
        /// True if the exchange should be deleted when the connection is closed
        /// </summary>
        bool AutoDelete { get; }

        /// <summary>
        /// Additional exchange arguments
        /// </summary>
        IDictionary<string, object> Arguments { get; }
    }
}
