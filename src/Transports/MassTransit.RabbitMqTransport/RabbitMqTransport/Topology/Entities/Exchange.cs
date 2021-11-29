namespace MassTransit.RabbitMqTransport.Topology
{
    using System.Collections.Generic;


    /// <summary>
    /// The exchange details used to declare the exchange to RabbitMQ
    /// </summary>
    public interface Exchange
    {
        /// <summary>
        /// The exchange name
        /// </summary>
        string ExchangeName { get; }

        /// <summary>
        /// The exchange type
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
        IDictionary<string, object> ExchangeArguments { get; }
    }
}
