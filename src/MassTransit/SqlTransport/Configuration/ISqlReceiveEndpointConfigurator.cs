#nullable enable
namespace MassTransit
{
    using System;
    using SqlTransport;


    /// <summary>
    /// Configure a database transport receive endpoint
    /// </summary>
    public interface ISqlReceiveEndpointConfigurator :
        IReceiveEndpointConfigurator,
        ISqlQueueEndpointConfigurator
    {
        /// <summary>
        /// The time to wait before the message is redelivered when faults are rethrown to the transport.
        /// Defaults to 0.
        /// </summary>
        TimeSpan? UnlockDelay { set; }

        /// <summary>
        /// Set number of concurrent messages per PartitionKey, higher value will increase throughput but will break delivery order (default: 1).
        /// This applies to the concurrent receive modes only.
        /// </summary>
        int ConcurrentDeliveryLimit { set; }

        /// <summary>
        /// Set the endpoint receive mode (changes the delivery behavior of messages to use partition keys, ordering, etc.
        /// </summary>
        /// <param name="mode"></param>
        /// <param name="concurrentDeliveryLimit"></param>
        void SetReceiveMode(SqlReceiveMode mode, int? concurrentDeliveryLimit = default);

        /// <summary>
        /// Adds a topic subscription to the receive endpoint by message type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        void Subscribe<T>(Action<ISqlTopicSubscriptionConfigurator>? callback = null)
            where T : class;

        /// <summary>
        /// Adds a topic subscription to the receive endpoint
        /// </summary>
        /// <param name="topicName">The topic name</param>
        /// <param name="callback">Configure the topic and the subscription</param>
        void Subscribe(string topicName, Action<ISqlTopicSubscriptionConfigurator>? callback = default);

        /// <summary>
        /// Add middleware to the receive endpoint <see cref="ClientContext" /> pipe
        /// </summary>
        /// <param name="configure"></param>
        void ConfigureClient(Action<IPipeConfigurator<ClientContext>>? configure);
    }
}
