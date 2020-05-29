namespace MassTransit.RabbitMqTransport.Topology.Specifications
{
    using System.Collections.Generic;
    using Builders;
    using Configurators;
    using GreenPipes;


    /// <summary>
    /// Used to bind an exchange to the sending
    /// </summary>
    public class ExchangeBindingPublishTopologySpecification :
        IRabbitMqPublishTopologySpecification
    {
        readonly bool _autoDelete;
        readonly IDictionary<string, object> _bindingArguments;
        readonly bool _durable;
        readonly IDictionary<string, object> _exchangeArguments;
        readonly string _exchangeType;
        readonly string _routingKey;

        public ExchangeBindingPublishTopologySpecification(ExchangeBindingConfigurator configurator)
        {
            ExchangeName = configurator.ExchangeName;
            _exchangeType = configurator.ExchangeType;
            _durable = configurator.Durable;
            _autoDelete = configurator.AutoDelete;
            _exchangeArguments = configurator.ExchangeArguments;
            _routingKey = configurator.RoutingKey;
            _bindingArguments = configurator.BindingArguments;
        }

        public string ExchangeName { get; }

        public IEnumerable<ValidationResult> Validate()
        {
            yield break;
        }

        public void Apply(IPublishEndpointBrokerTopologyBuilder builder)
        {
            var exchangeHandle = builder.ExchangeDeclare(ExchangeName, _exchangeType, _durable, _autoDelete, _exchangeArguments);

            var bindingHandle = builder.ExchangeBind(builder.Exchange, exchangeHandle, _routingKey, _bindingArguments);
        }
    }
}
