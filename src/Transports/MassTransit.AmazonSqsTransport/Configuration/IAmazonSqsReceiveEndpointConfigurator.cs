namespace MassTransit
{
    using System;
    using AmazonSqsTransport;


    /// <summary>
    /// Configure a receiving AmazonSQS endpoint
    /// </summary>
    public interface IAmazonSqsReceiveEndpointConfigurator :
        IReceiveEndpointConfigurator,
        IAmazonSqsQueueEndpointConfigurator
    {
        /// <summary>
        /// Bind an existing exchange for the message type to the receive endpoint by name
        /// </summary>
        /// <typeparam name="T"></typeparam>
        void Subscribe<T>(Action<IAmazonSqsTopicSubscriptionConfigurator> callback = null)
            where T : class;

        /// <summary>
        /// Bind an exchange to the receive endpoint exchange
        /// </summary>
        /// <param name="topicName">The exchange name</param>
        /// <param name="callback">Configure the exchange and binding</param>
        void Subscribe(string topicName, Action<IAmazonSqsTopicSubscriptionConfigurator> callback = default);

        void ConfigureClient(Action<IPipeConfigurator<ClientContext>> configure);

        void ConfigureConnection(Action<IPipeConfigurator<ConnectionContext>> configure);

        /// <summary>
        /// FIFO queues deliver messages to consumers partitioned by MessageGroupId, in SequenceNumber order. Calling this method will
        /// disable that behavior.
        /// </summary>
        void DisableMessageOrdering();
    }
}
