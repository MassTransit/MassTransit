namespace MassTransit
{
    using System;


    public interface IRabbitMqBusTopology :
        IBusTopology
    {
        new IRabbitMqPublishTopology PublishTopology { get; }

        new IRabbitMqSendTopology SendTopology { get; }

        new IRabbitMqMessagePublishTopology<T> Publish<T>()
            where T : class;

        new IRabbitMqMessageSendTopology<T> Send<T>()
            where T : class;

        /// <summary>
        /// Returns the destination address for the specified exchange
        /// </summary>
        /// <param name="exchangeName"></param>
        /// <param name="configure">Callback to configure exchange settings</param>
        /// <returns></returns>
        Uri GetDestinationAddress(string exchangeName, Action<IRabbitMqExchangeConfigurator> configure = null);

        /// <summary>
        /// Returns the destination address for the specified message type
        /// </summary>
        /// <param name="messageType">The message type</param>
        /// <param name="configure">Callback to configure exchange settings</param>
        /// <returns></returns>
        Uri GetDestinationAddress(Type messageType, Action<IRabbitMqExchangeConfigurator> configure = null);
    }
}
