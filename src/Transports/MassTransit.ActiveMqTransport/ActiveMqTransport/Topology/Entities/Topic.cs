namespace MassTransit.ActiveMqTransport.Topology
{
    /// <summary>
    /// The exchange details used to declare the exchange to ActiveMQ
    /// </summary>
    public interface Topic
    {
        /// <summary>
        /// The exchange name
        /// </summary>
        string EntityName { get; }

        /// <summary>
        /// True if the exchange should be durable, and survive a broker restart
        /// </summary>
        bool Durable { get; }

        /// <summary>
        /// True if the exchange should be deleted when the connection is closed
        /// </summary>
        bool AutoDelete { get; }
    }
}
