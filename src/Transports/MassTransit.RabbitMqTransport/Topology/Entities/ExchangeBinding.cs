namespace MassTransit.RabbitMqTransport.Topology.Entities
{
    using System.Collections.Generic;


    /// <summary>
    /// An exchange binding that goes to the consume exchange
    /// </summary>
    public interface ExchangeBinding
    {
        /// <summary>
        /// The source exchange
        /// </summary>
        Exchange Source { get; }

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
