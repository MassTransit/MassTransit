namespace MassTransit
{
    public interface IRabbitMqQueueEndpointConfigurator :
        IRabbitMqQueueConfigurator
    {
        /// <summary>
        /// Purge the messages from an existing queue on startup (note that upon reconnection to the server
        /// the queue will not be purged again, only when the service is restarted).
        /// </summary>
        bool PurgeOnStartup { set; }

        /// <summary>
        /// Sets the priority of the consumer (optional, no default value specified)
        /// </summary>
        int ConsumerPriority { set; }

        /// <summary>
        /// Should the consumer have exclusive access to the queue
        /// </summary>
        bool ExclusiveConsumer { set; }
    }
}
