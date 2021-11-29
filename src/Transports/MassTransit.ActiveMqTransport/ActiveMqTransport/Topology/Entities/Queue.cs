namespace MassTransit.ActiveMqTransport.Topology
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
        /// True if the queue should be deleted when the connection is closed
        /// </summary>
        bool AutoDelete { get; }
    }
}
