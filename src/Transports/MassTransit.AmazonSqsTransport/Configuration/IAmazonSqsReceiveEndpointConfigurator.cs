namespace MassTransit.AmazonSqsTransport
{
    using System;
    using GreenPipes;


    /// <summary>
    /// Configure a receiving AmazonSQS endpoint
    /// </summary>
    public interface IAmazonSqsReceiveEndpointConfigurator :
        IReceiveEndpointConfigurator,
        IQueueEndpointConfigurator
    {
        /// <summary>
        /// Use only if you use FIFO queue, this setting enables grouping by MessageGroupId and process messages in ordered way by SequenceNumber
        /// </summary>
        bool OrderedMessageHandlingEnabled { set; }

        /// <summary>
        /// Bind an existing exchange for the message type to the receive endpoint by name
        /// </summary>
        /// <typeparam name="T"></typeparam>
        void Subscribe<T>(Action<ITopicSubscriptionConfigurator> callback = null)
            where T : class;

        /// <summary>
        /// Bind an exchange to the receive endpoint exchange
        /// </summary>
        /// <param name="topicName">The exchange name</param>
        /// <param name="callback">Configure the exchange and binding</param>
        void Subscribe(string topicName, Action<ITopicSubscriptionConfigurator> callback = default);

        void ConfigureClient(Action<IPipeConfigurator<ClientContext>> configure);

        void ConfigureConnection(Action<IPipeConfigurator<ConnectionContext>> configure);
    }
}
