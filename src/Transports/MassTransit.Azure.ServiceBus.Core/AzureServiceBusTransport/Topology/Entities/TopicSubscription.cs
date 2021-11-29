namespace MassTransit.AzureServiceBusTransport.Topology
{
    /// <summary>
    /// A subscription that forwards to another topic
    /// </summary>
    public interface TopicSubscription
    {
        /// <summary>
        /// The source topic
        /// </summary>
        Topic Source { get; }

        /// <summary>
        /// The destination topic
        /// </summary>
        Topic Destination { get; }

        /// <summary>
        /// The subscription that binds them together
        /// </summary>
        Subscription Subscription { get; }
    }
}
