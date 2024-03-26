namespace MassTransit.RabbitMqTransport.Configuration
{
    using System;
    using System.Collections.Generic;
    using Topology;


    /// <summary>
    /// Used to bind an exchange to the consuming queue's exchange
    /// </summary>
    public class ExchangeToExchangeBindingConsumeTopologySpecification :
        RabbitMqExchangeBindingConfigurator,
        IRabbitMqExchangeToExchangeBindingConfigurator,
        IRabbitMqConsumeTopologySpecification
    {
        readonly List<IRabbitMqConsumeTopologySpecification> _specifications;

        public ExchangeToExchangeBindingConsumeTopologySpecification(string exchangeName, string exchangeType, bool durable = true, bool autoDelete = false)
            : base(exchangeName, exchangeType, durable, autoDelete)
        {
            _specifications = new List<IRabbitMqConsumeTopologySpecification>();
        }

        public IEnumerable<ValidationResult> Validate()
        {
            yield break;
        }

        public void Apply(IReceiveEndpointBrokerTopologyBuilder builder)
        {
            if (builder.BoundExchange == null)
                throw new ArgumentException("The builder should have an already bound exchange", nameof(builder));

            // save this, since it must be restored on exit
            var boundExchange = builder.BoundExchange;

            var exchangeHandle = builder.ExchangeDeclare(ExchangeName, ExchangeType, Durable, AutoDelete, ExchangeArguments);

            builder.ExchangeBind(exchangeHandle, boundExchange, RoutingKey, BindingArguments);

            builder.BoundExchange = exchangeHandle;

            foreach (var specification in _specifications)
                specification.Apply(builder);

            builder.BoundExchange = boundExchange;
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
