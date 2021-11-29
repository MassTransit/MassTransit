namespace MassTransit.RabbitMqTransport.Configuration
{
    using System.Collections.Generic;
    using Topology;


    /// <summary>
    /// Used to declare an exchange and queue, and bind them together.
    /// </summary>
    public class ExchangeToQueueBindingPublishTopologySpecification :
        QueueBindingConfigurator,
        IRabbitMqPublishTopologySpecification
    {
        public ExchangeToQueueBindingPublishTopologySpecification(string exchangeName, string exchangeType, string queueName = null, bool durable = true,
            bool autoDelete = false)
            : base(queueName ?? exchangeName, exchangeType, durable, autoDelete)
        {
            ExchangeName = exchangeName;
        }

        public IEnumerable<ValidationResult> Validate()
        {
            yield break;
        }

        public void Apply(IPublishEndpointBrokerTopologyBuilder builder)
        {
            var exchangeHandle = builder.ExchangeDeclare(ExchangeName, ExchangeType, Durable, AutoDelete, ExchangeArguments);

            var queueHandle = builder.QueueDeclare(QueueName, Durable, AutoDelete, Exclusive, QueueArguments);

            var bindingHandle = builder.QueueBind(exchangeHandle, queueHandle, RoutingKey, BindingArguments);
        }
    }
}
