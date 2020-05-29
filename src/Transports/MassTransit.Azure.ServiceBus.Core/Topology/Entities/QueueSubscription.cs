namespace MassTransit.Azure.ServiceBus.Core.Topology.Entities
{
    /// <summary>
    /// The exchange to queue binding details to declare the binding to RabbitMQ
    /// </summary>
    public interface QueueSubscription
    {
        /// <summary>
        /// The source exchange
        /// </summary>
        Topic Source { get; }

        /// <summary>
        /// The destination exchange
        /// </summary>
        Queue Destination { get; }

        /// <summary>
        /// The subscription that binds them
        /// </summary>
        Subscription Subscription { get; }
    }
}
