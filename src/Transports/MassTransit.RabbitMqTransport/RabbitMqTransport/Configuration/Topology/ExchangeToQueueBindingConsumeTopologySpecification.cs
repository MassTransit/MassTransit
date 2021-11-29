namespace MassTransit.RabbitMqTransport.Configuration
{
    using System.Collections.Generic;
    using Topology;


    /// <summary>
    /// Used to bind an exchange to the consuming queue's exchange
    /// </summary>
    public class ExchangeToQueueBindingConsumeTopologySpecification :
        QueueBindingConfigurator,
        IRabbitMqConsumeTopologySpecification
    {
        public ExchangeToQueueBindingConsumeTopologySpecification(string exchangeName, string exchangeType, string queueName = null, bool durable = true,
            bool autoDelete = false)
            : base(queueName ?? exchangeName, exchangeType, durable, autoDelete)
        {
            ExchangeName = exchangeName;
        }

        public IEnumerable<ValidationResult> Validate()
        {
            yield break;
        }

        public void Apply(IReceiveEndpointBrokerTopologyBuilder builder)
        {
            var exchangeHandle = builder.ExchangeDeclare(ExchangeName, ExchangeType, Durable, AutoDelete, ExchangeArguments);

            var queueHandle = builder.QueueDeclare(QueueName, Durable, AutoDelete, Exclusive, QueueArguments);

            var bindingHandle = builder.QueueBind(exchangeHandle, queueHandle, RoutingKey, BindingArguments);
        }
    }
}
