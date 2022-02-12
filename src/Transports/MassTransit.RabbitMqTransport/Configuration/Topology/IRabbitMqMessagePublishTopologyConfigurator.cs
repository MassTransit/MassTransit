#nullable enable
namespace MassTransit
{
    using System;


    public interface IRabbitMqMessagePublishTopologyConfigurator<TMessage> :
        IMessagePublishTopologyConfigurator<TMessage>,
        IRabbitMqMessagePublishTopology<TMessage>,
        IRabbitMqMessagePublishTopologyConfigurator
        where TMessage : class
    {
        /// <summary>
        /// Specifies the alternate exchange for the published message exchange, which is where messages are sent if no
        /// queues receive the message.
        /// </summary>
        string AlternateExchange { set; }

        /// <summary>
        /// Bind an exchange to a queue
        /// </summary>
        /// <param name="exchangeName"></param>
        /// <param name="queueName"></param>
        /// <param name="configure"></param>
        void BindQueue(string exchangeName, string? queueName, Action<IRabbitMqQueueBindingConfigurator>? configure = null);

        /// <summary>
        /// Bind an alternate exchange/queue for the published message type
        /// </summary>
        /// <param name="exchangeName"></param>
        /// <param name="queueName"></param>
        /// <param name="configure"></param>
        void BindAlternateExchangeQueue(string exchangeName, string? queueName = null, Action<IRabbitMqQueueBindingConfigurator>? configure = null);
    }


    public interface IRabbitMqMessagePublishTopologyConfigurator :
        IMessagePublishTopologyConfigurator,
        IRabbitMqExchangeConfigurator
    {
    }
}
