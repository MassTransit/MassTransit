#nullable enable
namespace MassTransit.InMemoryTransport
{
    using System.Collections.Generic;
    using System.Linq;
    using Configuration;
    using MassTransit.Configuration;
    using Transports.Fabric;


    public class InMemoryMessageConsumeTopology<TMessage> :
        MessageConsumeTopology<TMessage>,
        IInMemoryMessageConsumeTopologyConfigurator<TMessage>,
        IInMemoryMessageConsumeTopologyConfigurator
        where TMessage : class
    {
        readonly IMessageTopology<TMessage> _messageTopology;
        readonly IInMemoryPublishTopology _publishTopology;
        readonly IList<IInMemoryConsumeTopologySpecification> _specifications;

        public InMemoryMessageConsumeTopology(IMessageTopology<TMessage> messageTopology, IInMemoryPublishTopologyConfigurator publishTopology)
        {
            _messageTopology = messageTopology;
            _publishTopology = publishTopology;
            _specifications = new List<IInMemoryConsumeTopologySpecification>();
        }

        public void Apply(IMessageFabricConsumeTopologyBuilder builder)
        {
            foreach (var specification in _specifications)
                specification.Apply(builder);
        }

        public void Bind(ExchangeType? exchangeType, string? routingKey = default)
        {
            if (!IsBindableMessageType)
            {
                _specifications.Add(new InvalidInMemoryConsumeTopologySpecification(TypeCache<TMessage>.ShortName, "Is not a bindable message type"));
                return;
            }

            var bindExchangeType = exchangeType ?? _publishTopology.GetMessageTopology<TMessage>().ExchangeType;

            var specification = new ExchangeBindingConsumeTopologySpecification(_messageTopology.EntityName, bindExchangeType, routingKey);

            _specifications.Add(specification);
        }

        public override IEnumerable<ValidationResult> Validate()
        {
            return base.Validate().Concat(_specifications.SelectMany(x => x.Validate()));
        }
    }
}
