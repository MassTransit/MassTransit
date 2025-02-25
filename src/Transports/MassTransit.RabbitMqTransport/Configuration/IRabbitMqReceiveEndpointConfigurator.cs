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
        /// Bind an exchange to the receive endpoint exchange
        /// </summary>
        /// <param name="exchangeName">The exchange name</param>
        /// <param name="callback">Configure the exchange and binding</param>
        void Bind(string exchangeName, Action<IRabbitMqExchangeToExchangeBindingConfigurator> callback = null);

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
        /// Add middleware to the channel pipe
        /// </summary>
        /// <param name="configure"></param>
        void ConfigureChannel(Action<IPipeConfigurator<ChannelContext>> configure);

        /// <summary>
        /// Add middleware to the connection pipe
        /// </summary>
        /// <param name="configure"></param>
        void ConfigureConnection(Action<IPipeConfigurator<ConnectionContext>> configure);

        /// <summary>
        /// By default, RabbitMQ assigns a dynamically generated consumer tag, which is always the right choice. In certain scenarios
        /// where a specific consumer tag is needed, this will set it.
        /// </summary>
        /// <param name="consumerTag">The consumer tag to use for this receive endpoint.</param>
        void OverrideConsumerTag(string consumerTag);

        /// <summary>
        /// Configure receive endpoint to use a stream
        /// </summary>
        /// <param name="callback"></param>
        void Stream(Action<IRabbitMqStreamConfigurator> callback = null);

        /// <summary>
        /// Configure receive endpoint to use a stream
        /// </summary>
        /// <param name="consumerTag">Overrides the default consumer tag with the specified name</param>
        /// <param name="callback"></param>
        void Stream(string consumerTag, Action<IRabbitMqStreamConfigurator> callback = null);

        /// <summary>
        /// Configure the RabbitMQ delivery acknowledgement timeout for this queue explicitly. This is entirely optional,
        /// and generally not necessary.
        /// <see href="https://www.rabbitmq.com/docs/consumers#acknowledgement-timeout"/>
        /// </summary>
        /// <param name="timeSpan"></param>
        void SetDeliveryAcknowledgementTimeout(TimeSpan timeSpan);

        /// <summary>
        /// Configure the RabbitMQ delivery acknowledgement timeout for this queue explicitly. This is entirely optional,
        /// and generally not necessary.
        /// <see href="https://www.rabbitmq.com/docs/consumers#acknowledgement-timeout"/>
        /// </summary>
        /// <param name="d">days</param>
        /// <param name="h">hours</param>
        /// <param name="m">minutes</param>
        /// <param name="s">seconds</param>
        /// <param name="ms">milliseconds</param>
        void SetDeliveryAcknowledgementTimeout(int? d = null, int? h = null, int? m = null, int? s = null, int? ms = null);
    }
}
