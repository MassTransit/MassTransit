namespace MassTransit.RabbitMqTransport.Configuration
{
    using System.Collections.Generic;
    using Topology;


    /// <summary>
    /// Used to bind an exchange to the sending
    /// </summary>
    public class ExchangeBindingPublishTopologySpecification :
        RabbitMqExchangeBindingConfigurator,
        IRabbitMqPublishTopologySpecification
    {
        public ExchangeBindingPublishTopologySpecification(string exchangeName, string exchangeType, bool durable = true, bool autoDelete = false)
            : base(exchangeName, exchangeType, durable, autoDelete)
        {
        }

        public IEnumerable<ValidationResult> Validate()
        {
            yield break;
        }

        public void Apply(IPublishEndpointBrokerTopologyBuilder builder)
        {
            var exchangeHandle = builder.ExchangeDeclare(ExchangeName, ExchangeType, Durable, AutoDelete, ExchangeArguments);

            builder.ExchangeBind(builder.Exchange, exchangeHandle, RoutingKey, BindingArguments);
        }
    }
}
