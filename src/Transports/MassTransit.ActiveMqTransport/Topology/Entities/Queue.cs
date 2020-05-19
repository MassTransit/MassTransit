namespace MassTransit.ActiveMqTransport.Topology.Entities
{
    /// <summary>
    /// The queue details used to declare the queue to ActiveMQ
    /// </summary>
    public interface Queue
    {
        /// <summary>
        /// The queue name
        /// </summary>
        string EntityName { get; }

        /// <summary>
        /// True if the queue should be durable, and survive a broker restart
        /// </summary>
        bool Durable { get; }

        /// <summary>
        /// True if the queue should be deleted when the connection is closed
        /// </summary>
        bool AutoDelete { get; }
    }
}
