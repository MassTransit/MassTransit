namespace MassTransit.AmazonSqsTransport.Topology
{
    /// <summary>
    /// The topic to queue binding details to declare the binding to AmazonSQS
    /// </summary>
    public interface TopicSubscription
    {
        /// <summary>
        /// The topic
        /// </summary>
        Topic Source { get; }

        /// <summary>
        /// The queue
        /// </summary>
        Topic Destination { get; }
    }
}
