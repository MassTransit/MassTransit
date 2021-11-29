namespace MassTransit.RabbitMqTransport.Topology
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Configuration;


    public class RabbitMqMessageConsumeTopology<TMessage> :
        MessageConsumeTopology<TMessage>,
        IRabbitMqMessageConsumeTopologyConfigurator<TMessage>,
        IRabbitMqMessageConsumeTopologyConfigurator
        where TMessage : class
    {
        readonly IMessageTopology<TMessage> _messageTopology;
        readonly IRabbitMqMessagePublishTopology<TMessage> _publishTopology;
        readonly IList<IRabbitMqConsumeTopologySpecification> _specifications;

        public RabbitMqMessageConsumeTopology(IMessageTopology<TMessage> messageTopology, IMessageExchangeTypeSelector<TMessage> exchangeTypeSelector,
            IRabbitMqMessagePublishTopology<TMessage> publishTopology)
        {
            _messageTopology = messageTopology;
            _publishTopology = publishTopology;
            ExchangeTypeSelector = exchangeTypeSelector;

            _specifications = new List<IRabbitMqConsumeTopologySpecification>();
        }

        IMessageExchangeTypeSelector<TMessage> ExchangeTypeSelector { get; }

        public void Apply(IReceiveEndpointBrokerTopologyBuilder builder)
        {
            foreach (var specification in _specifications)
                specification.Apply(builder);
        }

        public void Bind(Action<IRabbitMqExchangeBindingConfigurator> configure = null)
        {
            if (!IsBindableMessageType)
            {
                _specifications.Add(new InvalidRabbitMqConsumeTopologySpecification(TypeCache<TMessage>.ShortName, "Is not a bindable message type"));
                return;
            }

            var specification = new ExchangeBindingConsumeTopologySpecification(_publishTopology.Exchange);

            configure?.Invoke(specification);

            _specifications.Add(specification);
        }

        public override IEnumerable<ValidationResult> Validate()
        {
            return base.Validate().Concat(_specifications.SelectMany(x => x.Validate()));
        }
    }
}
