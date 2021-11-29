namespace MassTransit.ActiveMqTransport
{
    public interface EntitySettings
    {
        /// <summary>
        /// The entity name (either a topic or a queue)
        /// </summary>
        string EntityName { get; }

        /// <summary>
        /// True if messages should be persisted to disk for the queue
        /// </summary>
        bool Durable { get; }

        /// <summary>
        /// True if the queue/exchange should automatically be deleted
        /// </summary>
        bool AutoDelete { get; }
    }
}
