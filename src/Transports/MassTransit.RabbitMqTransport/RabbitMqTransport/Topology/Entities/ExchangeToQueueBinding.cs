namespace MassTransit.RabbitMqTransport.Topology
{
    using System.Collections.Generic;


    /// <summary>
    /// The exchange to queue binding details to declare the binding to RabbitMQ
    /// </summary>
    public interface ExchangeToQueueBinding
    {
        /// <summary>
        /// The source exchange
        /// </summary>
        Exchange Source { get; }

        /// <summary>
        /// The destination exchange
        /// </summary>
        Queue Destination { get; }

        /// <summary>
        /// A routing key for the exchange binding
        /// </summary>
        string RoutingKey { get; }

        /// <summary>
        /// The arguments for the binding
        /// </summary>
        IDictionary<string, object> Arguments { get; }
    }
}
