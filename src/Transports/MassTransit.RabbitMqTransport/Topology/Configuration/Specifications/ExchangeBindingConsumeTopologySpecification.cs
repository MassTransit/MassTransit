namespace MassTransit.RabbitMqTransport.Topology.Specifications
{
    using System.Collections.Generic;
    using Builders;
    using Configurators;
    using Entities;
    using GreenPipes;


    /// <summary>
    /// Used to bind an exchange to the consuming queue's exchange
    /// </summary>
    public class ExchangeBindingConsumeTopologySpecification :
        ExchangeBindingConfigurator,
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
            var exchangeHandle = builder.ExchangeDeclare(ExchangeName, ExchangeType, Durable, AutoDelete, ExchangeArguments);

            var bindingHandle = builder.ExchangeBind(exchangeHandle, builder.Exchange, RoutingKey, BindingArguments);
        }
    }
}
