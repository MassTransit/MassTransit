namespace MassTransit.RabbitMqTransport.Configuration
{
    using System.Collections.Generic;
    using Topology;


    /// <summary>
    /// Used to bind an exchange to the consuming queue's exchange
    /// </summary>
    public class ExchangeBindingConsumeTopologySpecification :
        RabbitMqExchangeBindingConfigurator,
        IRabbitMqConsumeTopologySpecification
    {
        public ExchangeBindingConsumeTopologySpecification(string exchangeName, string exchangeType, bool durable = true, bool autoDelete = false)
            : base(exchangeName, exchangeType, durable, autoDelete)
        {
        }

        public ExchangeBindingConsumeTopologySpecification(Exchange exchange)
            : base(exchange)
        {
        }

        public IEnumerable<ValidationResult> Validate()
        {
            yield break;
        }

        public void Apply(IReceiveEndpointBrokerTopologyBuilder builder)
        {
            RabbitMqExchangeBindingConfigurator exchangeBindingConfigurator = this;
            ExchangeHandle destination = builder.Exchange;
            do
            {
                var source = builder.ExchangeDeclare(exchangeBindingConfigurator.ExchangeName, exchangeBindingConfigurator.ExchangeType, exchangeBindingConfigurator.Durable, exchangeBindingConfigurator.AutoDelete, exchangeBindingConfigurator.ExchangeArguments);

                var bindingHandle = builder.ExchangeBind(source, destination, exchangeBindingConfigurator.RoutingKey, exchangeBindingConfigurator.BindingArguments);

                destination = source;

                exchangeBindingConfigurator = exchangeBindingConfigurator.ExchangeBindingConfigurator as RabbitMqExchangeBindingConfigurator;
            }
            while (exchangeBindingConfigurator != null);
        }
    }
}
