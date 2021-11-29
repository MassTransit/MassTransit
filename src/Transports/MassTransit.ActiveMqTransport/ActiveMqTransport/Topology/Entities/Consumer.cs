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
    }
}
