namespace MassTransit.AmazonSqsTransport.Configuration
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
        /// If true, creates message consumers for the message types in consumers, handlers, etc.
        /// With AmazonSQS, these are virtual consumers tied to the virtual topics
        /// </summary>
        bool SubscribeMessageTopics { set; }

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
        void Subscribe(string topicName, Action<ITopicSubscriptionConfigurator> callback);

        void ConfigureClient(Action<IPipeConfigurator<ClientContext>> configure);

        void ConfigureConnection(Action<IPipeConfigurator<ConnectionContext>> configure);
    }
}
