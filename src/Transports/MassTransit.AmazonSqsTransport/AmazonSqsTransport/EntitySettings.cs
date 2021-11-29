namespace MassTransit.AmazonSqsTransport
{
    using System.Collections.Generic;


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

        /// <summary>
        /// Collection of tags to assign to queue when created.
        /// </summary>
        IDictionary<string, string> Tags { get; }
    }
}
