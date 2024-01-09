#nullable enable
namespace MassTransit.SqlTransport.Topology
{
    /// <summary>
    /// The exchange to exchange binding details to declare the binding to RabbitMQ
    /// </summary>
    public interface TopicToTopicSubscription
    {
        /// <summary>
        /// The source exchange
        /// </summary>
        Topic Source { get; }

        /// <summary>
        /// The destination exchange
        /// </summary>
        Topic Destination { get; }

        SqlSubscriptionType SubscriptionType { get; }

        /// <summary>
        /// A routing key for the exchange binding
        /// </summary>
        string? RoutingKey { get; }
    }
}
