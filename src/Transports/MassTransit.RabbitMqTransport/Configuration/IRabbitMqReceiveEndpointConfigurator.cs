namespace MassTransit
{
    using System;
    using RabbitMqTransport;


    /// <summary>
    /// Configure a receiving RabbitMQ endpoint
    /// </summary>
    public interface IRabbitMqReceiveEndpointConfigurator :
        IReceiveEndpointConfigurator,
        IRabbitMqQueueEndpointConfigurator
    {
        /// <summary>
        /// If false, deploys only exchange, without queue
        /// </summary>
        bool BindQueue { set; }

        /// <summary>
        /// Specifies the dead letter exchange name, which is used to send expired messages
        /// </summary>
        string DeadLetterExchange { set; }

        /// <summary>
        /// Configure a management endpoint for this receive endpoint
        /// </summary>
        /// <param name="management"></param>
        void ConnectManagementEndpoint(IReceiveEndpointConfigurator management);

        /// <summary>
        /// Bind an exchange to the receive endpoint exchange
        /// </summary>
        /// <param name="exchangeName">The exchange name</param>
        /// <param name="callback">Configure the exchange and binding</param>
        void Bind(string exchangeName, Action<IRabbitMqExchangeBindingConfigurator> callback = null);

        /// <summary>
        /// Bind an exchange to the receive endpoint exchange
        /// </summary>
        /// <param name="callback">Configure the exchange and binding</param>
        void Bind<T>(Action<IRabbitMqExchangeBindingConfigurator> callback = null)
            where T : class;

        /// <summary>
        /// Bind a dead letter exchange and queue to the receive endpoint so that expired messages are moved automatically.
        /// </summary>
        /// <param name="exchangeName"></param>
        /// <param name="queueName"></param>
        /// <param name="configure"></param>
        void BindDeadLetterQueue(string exchangeName, string queueName = null, Action<IRabbitMqQueueBindingConfigurator> configure = null);

        /// <summary>
        /// Add middleware to the model pipe
        /// </summary>
        /// <param name="configure"></param>
        void ConfigureModel(Action<IPipeConfigurator<ModelContext>> configure);

        /// <summary>
        /// Add middleware to the connection pipe
        /// </summary>
        /// <param name="configure"></param>
        void ConfigureConnection(Action<IPipeConfigurator<ConnectionContext>> configure);
    }
}
