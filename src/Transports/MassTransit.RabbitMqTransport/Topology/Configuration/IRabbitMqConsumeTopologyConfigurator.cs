namespace MassTransit.RabbitMqTransport.Topology
{
    using System;
    using MassTransit.Topology;


    public interface IRabbitMqConsumeTopologyConfigurator :
        IConsumeTopologyConfigurator,
        IRabbitMqConsumeTopology
    {
        new IRabbitMqMessageConsumeTopologyConfigurator<T> GetMessageTopology<T>()
            where T : class;

        void AddSpecification(IRabbitMqConsumeTopologySpecification specification);

        /// <summary>
        /// Bind an exchange, using the configurator
        /// </summary>
        /// <param name="exchangeName"></param>
        /// <param name="configure"></param>
        void Bind(string exchangeName, Action<IExchangeBindingConfigurator> configure = null);

        /// <summary>
        /// Bind an exchange to a queue, both of which are declared if they do not exist. Useful
        /// for creating alternate/dead-letter exchanges and queues for messages
        /// </summary>
        /// <param name="exchangeName">The exchange name to bind</param>
        /// <param name="queueName">The queue name to declare/bind to the exchange</param>
        /// <param name="configure">The configuration callback</param>
        void BindQueue(string exchangeName, string queueName, Action<IQueueBindingConfigurator> configure = null);
    }
}
