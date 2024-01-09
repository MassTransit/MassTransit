#nullable enable
namespace MassTransit.SqlTransport.Topology
{
    /// <summary>
    /// The exchange to queue binding details to declare the binding to RabbitMQ
    /// </summary>
    public interface TopicToQueueSubscription
    {
        /// <summary>
        /// The source exchange
        /// </summary>
        Topic Source { get; }

        /// <summary>
        /// The destination exchange
        /// </summary>
        Queue Destination { get; }

        SqlSubscriptionType SubscriptionType { get; }

        /// <summary>
        /// A routing key for the exchange binding
        /// </summary>
        string? RoutingKey { get; }
    }
}
