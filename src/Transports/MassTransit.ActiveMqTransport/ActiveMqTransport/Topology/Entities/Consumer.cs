namespace MassTransit.ActiveMqTransport.Topology
{
    /// <summary>
    /// The exchange to queue binding details to declare the binding to ActiveMQ
    /// </summary>
    public interface Consumer
    {
        /// <summary>
        /// The virtual topic
        /// </summary>
        Topic Source { get; }

        /// <summary>
        /// The virtual topic consumer
        /// </summary>
        Queue Destination { get; }

        /// <summary>
        /// A routing key for the exchange binding
        /// </summary>
        string Selector { get; }

        /// <summary>
        /// The consumer name
        /// </summary>
        string ConsumerName { get; }

        /// <summary>
        /// True if the consumer is shared. 
        /// </summary>
        /// <remarks>
        /// When you have multiple consumers on the same topic with same <see cref="ConsumerName"/>, you can use a shared consumer to load balance messages between the consumers.
        /// </remarks>
        bool IsShared { get; }
    }
}
