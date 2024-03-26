namespace MassTransit.RabbitMqTransport.Configuration
{
    using System;
    using System.Collections.Generic;
    using Topology;


    /// <summary>
    /// Used to bind an exchange to the consuming queue's exchange
    /// </summary>
    public class ExchangeBindingConsumeTopologySpecification :
        RabbitMqExchangeBindingConfigurator,
        IRabbitMqExchangeToExchangeBindingConfigurator,
        IRabbitMqConsumeTopologySpecification
    {
        readonly List<IRabbitMqConsumeTopologySpecification> _specifications;

        public ExchangeBindingConsumeTopologySpecification(string exchangeName, string exchangeType, bool durable = true, bool autoDelete = false)
            : base(exchangeName, exchangeType, durable, autoDelete)
        {
            _specifications = new List<IRabbitMqConsumeTopologySpecification>();
        }

        public ExchangeBindingConsumeTopologySpecification(Exchange exchange)
            : base(exchange)
        {
            _specifications = new List<IRabbitMqConsumeTopologySpecification>();
        }

        public IEnumerable<ValidationResult> Validate()
        {
            yield break;
        }

        public void Apply(IReceiveEndpointBrokerTopologyBuilder builder)
        {
            var exchangeHandle = builder.ExchangeDeclare(ExchangeName, ExchangeType, Durable, AutoDelete, ExchangeArguments);

            builder.ExchangeBind(exchangeHandle, builder.Exchange, RoutingKey, BindingArguments);

            builder.BoundExchange = exchangeHandle;

            foreach (var specification in _specifications)
                specification.Apply(builder);
        }

        public void Bind(string exchangeName, Action<IRabbitMqExchangeToExchangeBindingConfigurator> configure)
        {
            if (string.IsNullOrWhiteSpace(exchangeName))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(exchangeName));

            var specification =
                new ExchangeToExchangeBindingConsumeTopologySpecification(exchangeName, ExchangeType, Durable, AutoDelete) { RoutingKey = RoutingKey };

            configure?.Invoke(specification);

            _specifications.Add(specification);
        }
    }
}
