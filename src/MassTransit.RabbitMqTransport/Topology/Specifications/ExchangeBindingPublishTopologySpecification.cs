namespace MassTransit.RabbitMqTransport.Topology.Specifications
{
    using System.Collections.Generic;
    using Configurators;
    using GreenPipes;


    /// <summary>
    /// Used to bind an exchange to the sending 
    /// </summary>
    public class ExchangeBindingPublishTopologySpecification :
        IRabbitMqPublishTopologySpecification
    {
        readonly string _exchangeName;
        readonly string _exchangeType;
        readonly bool _durable;
        readonly bool _autoDelete;
        readonly IDictionary<string, object> _exchangeArguments;
        readonly string _routingKey;
        readonly IDictionary<string, object> _bindingArguments;

        public ExchangeBindingPublishTopologySpecification(ExchangeBindingConfigurator configurator)
        {
            _exchangeName = configurator.ExchangeName;
            _exchangeType = configurator.ExchangeType;
            _durable = configurator.Durable;
            _autoDelete = configurator.AutoDelete;
            _exchangeArguments = configurator.ExchangeArguments;
            _routingKey = configurator.RoutingKey;
            _bindingArguments = configurator.BindingArguments;
        }

        public string ExchangeName => _exchangeName;

        public IEnumerable<ValidationResult> Validate()
        {
            yield break;
        }

        public void Apply(IRabbitMqPublishTopologyBuilder builder)
        {
            var exchangeHandle = builder.ExchangeDeclare(_exchangeName, _exchangeType, _durable, _autoDelete, _exchangeArguments);

            var bindingHandle = builder.ExchangeBind(builder.Exchange, exchangeHandle, _routingKey, _bindingArguments);
        }
    }
}