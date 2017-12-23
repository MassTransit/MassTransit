namespace MassTransit.RabbitMqTransport
{
    public interface IQueueEndpointConfigurator :
        IQueueConfigurator
    {
        /// <summary>
        /// Specify the maximum number of concurrent messages that are consumed
        /// </summary>
        /// <value>The limit</value>
        ushort PrefetchCount { set; }

        /// <summary>
        /// Purge the messages from an existing queue on startup (note that upon reconnection to the server
        /// the queue will not be purged again, only when the service is restarted).
        /// </summary>
        bool PurgeOnStartup { set; }
    }
}