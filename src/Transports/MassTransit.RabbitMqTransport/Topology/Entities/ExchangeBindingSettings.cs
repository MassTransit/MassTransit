namespace MassTransit.RabbitMqTransport.Topology.Entities
{
    using System.Collections.Generic;


    /// <summary>
    /// Settings for a subscription to be bound into the RabbitMQ exchanges
    /// </summary>
    public interface ExchangeBindingSettings
    {
        /// <summary>
        /// The exchange settings
        /// </summary>
        ExchangeSettings Exchange { get; }

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
