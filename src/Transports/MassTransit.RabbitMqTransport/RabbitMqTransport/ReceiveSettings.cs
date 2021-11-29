namespace MassTransit.RabbitMqTransport
{
    using System;
    using System.Collections.Generic;


    /// <summary>
    /// Specify the receive settings for a receive transport
    /// </summary>
    public interface ReceiveSettings :
        EntitySettings
    {
        /// <summary>
        /// The queue name to receive from
        /// </summary>
        string QueueName { get; }

        /// <summary>
        /// The number of unacknowledged messages to allow to be processed concurrently
        /// </summary>
        ushort PrefetchCount { get; }

        /// <summary>
        /// True if the queue receive should be exclusive and not shared
        /// </summary>
        bool Exclusive { get; }

        /// <summary>
        /// Arguments passed to QueueDeclare
        /// </summary>
        IDictionary<string, object> QueueArguments { get; }

        string RoutingKey { get; }

        IDictionary<string, object> BindingArguments { get; }

        /// <summary>
        /// If True, and a queue name is specified, if the queue exists and has messages, they are purged at startup
        /// If the connection is reset, messages are not purged until the service is reset
        /// </summary>
        bool PurgeOnStartup { get; }

        /// <summary>
        /// Arguments passed to the basicConsume
        /// </summary>
        IDictionary<string, object> ConsumeArguments { get; }

        /// <summary>
        /// Should the consumer have exclusive access to the queue
        /// </summary>
        bool ExclusiveConsumer { get; }

        /// <summary>
        /// When the queue should expire
        /// </summary>
        TimeSpan? QueueExpiration { get; }

        /// <summary>
        /// If false, deploys only exchange, without queue
        /// </summary>
        bool BindQueue { get; }

        /// <summary>
        /// False by default, True if no acks are supported.
        /// </summary>
        bool NoAck { get; }

        /// <summary>
        /// Get the input address for the transport on the specified host
        /// </summary>
        Uri GetInputAddress(Uri hostAddress);
    }
}
